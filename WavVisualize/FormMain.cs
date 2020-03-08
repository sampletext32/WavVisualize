using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WavVisualize
{
    public partial class FormMain : Form
    {
        #region Data

        //плеер
        private WindowsMediaPlayerProvider _playerProvider;

        //текущий открытый Wav файл
        private WavFileData _currentWavFileData;
        
        private FFTProvider _fftProvider;

        private VolumeProvider _volumeProvider;

        private VolumeDrawer _volumeDrawer;

        private SpectrumDrawer _spectrumDrawer;

        private SpectrumDiagramDrawer _spectrumDiagramDrawer;

        public int TrimFrequency = 20000;

        //создавать ли волну последовательно
        public readonly bool CreateWaveformSequentially = true;

        //количество потоков для создания волны (-1, чтобы главный поток рисовал без зависаний)
        public readonly int ThreadsForWaveformCreation = Environment.ProcessorCount / 2;

        //частота пропуска сэмплов при создании волны
        public readonly int WaveformSkipSampleRate = 0;

        //сколько раз в секунду обновляется состояние плеера
        public int UpdateRate = 60;

        //коэффициент смягчения резких скачков
        public float EasingCoef = 0.1f;

        //ширина столбиков громкости
        public readonly int VolumeBandWidth = 20;

        //высота цифровых кусочков
        public readonly int DigitalPieceHeight = 2;

        //расстояние между цифровыми кусочками
        public readonly int DistanceBetweenBands = 1;

        //общая высота спектра
        public float SpectrumHeight;

        //нужно ли рисовать цифровые столбики
        public bool DisplayDigital = true;

        //линия 0 отрисовки спектра
        public float SpectrumBaselineY;

        //количество кусочков цифрового столбика
        public int DigitalBandPiecesCount;

        //количество столбиков спектра
        public readonly int TotalSpectrumBands = 100;

        //общая ширина спектра
        public int TotalSpectrumWidth;

        //сколько сэмплов идёт на преобразование спектра (обязательно степень двойки)
        public int SpectrumUseSamples = 4096;

        //нажата ли сейчас мышь на волне
        public bool PressedOnWaveform;

        public bool ApplyTimeThinning = true;

        public int FramesProcessed;

        public float ScaleX = 1f;

        private Dictionary<string, object> _waveformParameters;

        private DirectBitmap _waveformBitmap;

        #endregion

        #region Form Load

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            _waveformBitmap = new DirectBitmap(pictureBoxWaveform.Width, pictureBoxWaveform.Height);

            SetPlayerProvider();
            SetFFTProvider();
            SetVolumeDrawer();
            SetSpectrumDrawer();

            FileLoader.OnBeginMp3Decompression += () => { SetLabelStatusText("Begin Mp3 Decompression"); };
            FileLoader.OnBeginWavWriting += () => { SetLabelStatusText("Begin Wav Writing"); };

            Application.Idle += OnApplicationIdle;

            //выводим коэффициенты на форму
            numericUpDownEasing.Value = (int) (EasingCoef * 10);

            numericUpDownPow2Spectrum.Value = FastPowLog2Provider.FastLog2(SpectrumUseSamples);

            trackBarTrimFrequency.Value = TrimFrequency;

            checkBoxApplyTimeThinning.Checked = ApplyTimeThinning;

            labelMaxFrequency.Text = "Max Frequency: " + TrimFrequency;
        }

        #endregion

        #region Timers

        //шаг обновления
        private void timerUpdater_Tick(object sender, EventArgs e)
        {
            labelStatus.Text = _playerProvider.GetPlayState().ToString();
            
            float currentPosition = _playerProvider.GetElapsedSeconds();
            float duration = _playerProvider.GetDurationSeconds();

            Tuple<int, int, int> currentTime = TimeProvider.SecondsAsTime(currentPosition);
            Tuple<int, int, int> durationTime = TimeProvider.SecondsAsTime(duration);

            labelElapsed.Text =
                $@"{currentTime.Item1:00} : {currentTime.Item2:00} : {currentTime.Item3:00} / {
                    durationTime.Item1:00} : {durationTime.Item2:00} : {durationTime.Item3:00}";

            //вызываем перерисовку волны и спектра
            pictureBoxWaveform.Refresh();
            pictureBoxRealtimeSpectrum.Refresh();
            pictureBoxVolume.Refresh();
            pictureBoxSpectrumDiagram.Refresh();
            FramesProcessed++;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelFPS.Text = "FPS: " + FramesProcessed;
            FramesProcessed = 0;
        }

        #endregion

        #region Core Setups

        private void SetPlayerProvider()
        {
            _playerProvider = new WindowsMediaPlayerProvider();
            //_playerProvider.OnPlayEnd += () => { timerUpdater.Stop(); };
            //_playerProvider.OnPlayStart += () => { timerUpdater.Start(); };
        }

        private void SetVolumeProvider()
        {
            _volumeProvider =
                new MaxInRegionVolumeProvider(_currentWavFileData.LeftChannel, _currentWavFileData.RightChannel,
                    _currentWavFileData.sampleRate / UpdateRate);
        }

        private void SetWaveformProvider()
        {
            _waveformParameters = new Dictionary<string, object>();

            _waveformBitmap.Clear();

            _waveformParameters["mode"] = 1;
            _waveformParameters["directBitmap"] = _waveformBitmap;
            _waveformParameters["leftColor"] = (int) (0x7cfc00 | (0xFF << 24)); //LawnGreen
            _waveformParameters["rightColor"] = (int) (0xff4500 | (0xFF << 24)); //OrangeRed
            _waveformParameters["leftChannel"] = _currentWavFileData.LeftChannel;
            _waveformParameters["rightChannel"] = _currentWavFileData.RightChannel;
            _waveformParameters["samplesCount"] = _currentWavFileData.samplesCount;
            _waveformParameters["verticalScale"] = 0.9f;
            _waveformParameters["degreeOfParallelism"] = Environment.ProcessorCount;

            new TrueWaveformProvider().RecreateAsync(_waveformParameters);
        }

        private void SetFFTProvider()
        {
            _fftProvider = new CorrectCooleyTukeyInPlaceFFTProvider(SpectrumUseSamples, ApplyTimeThinning);
        }

        private void SetVolumeDrawer()
        {
            _volumeDrawer = new DigitalVolumeDrawer(Rectangle.FromPictureBox(pictureBoxVolume), Color.LawnGreen,
                Color.OrangeRed, 50, 1);
        }

        private void SetSpectrumDrawer()
        {
            _spectrumDrawer = new TopLineSpectrumDrawer(SpectrumUseSamples, 10f,
                Rectangle.FromPictureBox(pictureBoxRealtimeSpectrum), Color.OrangeRed);

            //_spectrumDrawer = new DigitalBandSpectrumDrawer(SpectrumUseSamples, 10f,
            //    Rectangle.FromPictureBox(pictureBoxRealtimeSpectrum), Color.OrangeRed, TotalSpectrumBands,
            //    DistanceBetweenBands, 70, 1);

            _spectrumDrawer.SetTrimmingFrequency(TrimFrequency);
            _spectrumDrawer.SetApplyTimeThinning(ApplyTimeThinning);

            if (_currentWavFileData != null)
            {
                _spectrumDrawer.LoadSpectrum(_currentWavFileData.GetSpectrumForPosition(
                        _playerProvider.GetNormalizedPosition(),
                        _fftProvider), 1 - EasingCoef);
            }
        }

        public void SetSpectrumDiagramDrawer()
        {
            _spectrumDiagramDrawer?.Cancel();
            //_spectrumDiagramDrawer = new IterationableSpectrumDiagramDrawer(SpectrumUseSamples, Rectangle.FromPictureBox(pictureBoxSpectrumDiagram), _currentWavFileData, 50);
            _spectrumDiagramDrawer = new BasicSpectrumDiagramDrawer(SpectrumUseSamples,
                Rectangle.FromPictureBox(pictureBoxSpectrumDiagram), _currentWavFileData);
            _spectrumDiagramDrawer.SetTrimmingFrequency(TrimFrequency);
            _spectrumDiagramDrawer.SetApplyTimeThinning(ApplyTimeThinning);
            _spectrumDiagramDrawer.Recreate();
        }

        #endregion

        #region Helper Methods

        private void SetLabelStatusText(string text)
        {
            if (labelStatus.InvokeRequired)
            {
                Invoke(new Action(() => { labelStatus.Text = text; }));
            }
            else
            {
                labelStatus.Text = text;
            }
        }

        async void OpenFile()
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Файлы Audio (*.wav, *.mp3)|*.wav;*.mp3";
            if (opf.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string filename = opf.FileName;

            SetLabelStatusText("Opening");

            byte[] fileData = await FileLoader.LoadAny(filename);

            //читаем Wav файл
            var wavFileData = await WavFileData.LoadWavFile(fileData);

            SetLabelStatusText("Playing");

            this.Text = opf.SafeFileName;

            _currentWavFileData = wavFileData;

            SetWaveformProvider();

            SetVolumeProvider();

            //создаём новый медиафайл
            _playerProvider.SetFile(filename);

            //SetSpectrumDrawer();
            SetSpectrumDiagramDrawer();

            //по невероятной причине, из-за открытия диалогового окна, форма находится не в фокусе
            //поэтому выводим форму на первый план
            this.BringToFront();
        }

        private void OnApplicationIdle(object sender, EventArgs e)
        {
            timerUpdater.Stop();
            while (NativeMethods.AppIsIdle())
            {
                timerUpdater_Tick(null, null);
            }

            timerUpdater.Start();
        }


        private void DrawCaret(Graphics g, int x, int height, bool top = false)
        {
            g.FillRectangle(Brushes.Black, x, 0, 1, height);

            int caretWidth = 20;
            int caretHeight = 5;

            int caretStartX = Math.Max(x - caretWidth / 2, 0);
            int caretEndX = Math.Min(x + caretWidth / 2, pictureBoxWaveform.Width);
            int caretDrawWidth = caretEndX - caretStartX;
            int caretY;

            if (top)
            {
                caretY = 0;
            }
            else
            {
                caretY = height - caretHeight;
            }

            g.FillRectangle(Brushes.DarkGray, caretStartX, caretY, caretDrawWidth, caretHeight);
        }

        #endregion

        #region Paint Events

        //перерисовка волны
        private void pictureBoxWaveform_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_waveformBitmap.Bitmap, 0, 0);

            //рисуем вертикальную линию текущей позиции = нормализованная позиция воспроизведения * ширину поля
            var x = (int) (_playerProvider.GetNormalizedPosition() * pictureBoxWaveform.Width);

            DrawCaret(e.Graphics, x, pictureBoxWaveform.Height, false);
        }

        //шаг отрисовки спектра
        private void pictureBoxSpectrum_Paint(object sender, PaintEventArgs e)
        {
            if (_playerProvider.IsPlaying() || _playerProvider.IsPaused()) //если сейчас воспроизводится
            {
                float normalized = _playerProvider.GetNormalizedPosition();
                //на каком сейчас сэмпле находимся
                int currentSample = (int) (normalized * _currentWavFileData.samplesCount);

                //если начало участка меньше чем количество сэмплов - сэмплов на преобразование спектра (можно вместить ещё раз рассчитать спектр)
                if (currentSample < _currentWavFileData.samplesCount - SpectrumUseSamples && currentSample >= 0)
                {
                    //рисуем спектр
                    float[] spectrum = _currentWavFileData.GetSpectrumForPosition(normalized, _fftProvider);
                    _spectrumDrawer.LoadSpectrum(spectrum, 1 - EasingCoef);
                    _spectrumDrawer.Draw(e.Graphics);
                }
            }
        }

        private void pictureBoxVolume_Paint(object sender, PaintEventArgs e)
        {
            if (_playerProvider.IsPlaying() || _playerProvider.IsPaused()) //если сейчас воспроизводится
            {
                float normalized = _playerProvider.GetNormalizedPosition();
                //на каком сейчас сэмпле находимся
                int currentSample = (int) (normalized * _currentWavFileData.samplesCount);

                //длина участка сэмплов, на котором измеряем громкость
                int regionLength =
                    _currentWavFileData.sampleRate / UpdateRate; //_currentWavFileData.sampleRate / UpdateRate;

                //если начало участка меньше чем количество сэплов - длина участка (можно вместить ещё участок)
                if (currentSample < _currentWavFileData.samplesCount - regionLength && currentSample >= 0)
                {
                    _volumeProvider.Calculate(currentSample);

                    _volumeDrawer.LoadVolume(_volumeProvider.GetL(), _volumeProvider.GetR(), (1 - EasingCoef));
                    _volumeDrawer.Draw(e.Graphics);
                }
            }
        }

        private void pictureBoxSpectrumDiagram_Paint(object sender, PaintEventArgs e)
        {
            //рисуем вертикальную линию текущей позиции = нормализованная позиция воспроизведения * ширину поля
            var x = (int) (_playerProvider.GetNormalizedPosition() * pictureBoxWaveform.Width);

            _spectrumDiagramDrawer?.Draw(e.Graphics);

            DrawCaret(e.Graphics, x, pictureBoxSpectrumDiagram.Height, true);
        }

        #endregion

        #region Main Buttons Click Handlers

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void buttonPlayPause_Click(object sender, EventArgs e)
        {
            if (!_playerProvider.IsPlaying())
            {
                _playerProvider.Play();
            }
            else
            {
                _playerProvider.Pause();
            }
        }

        #endregion

        #region Mouse Scroll Handlers

        //нажатие на волну
        private void pictureBoxPlot_MouseDown(object sender, MouseEventArgs e)
        {
            PressedOnWaveform = true; //сохраняем флаг

            //выставляем позицию воспроизведения, как нормализацию координаты клика * длительность трека
            _playerProvider.SetNormalizedPosition((float) e.X / pictureBoxWaveform.Width);
        }

        //когда двигаем мышь по волне
        private void pictureBoxPlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (PressedOnWaveform) //если нажата кнопка
            {
                //выставляем позицию воспроизведения, как нормализацию координаты клика * длительность трека
                _playerProvider.SetNormalizedPosition((float) e.X / pictureBoxWaveform.Width);

                //вызываем перерисовку, т.к. сдвинулась позиция
                pictureBoxWaveform.Refresh();
            }
        }

        //отпустили мышь над волной
        private void pictureBoxPlot_MouseUp(object sender, MouseEventArgs e)
        {
            PressedOnWaveform = false;
        }

        #endregion

        #region User Realtime-Controllable Tweakers Handlers

        //обработка изменения смягчающего коэффициента
        private void numericUpDownEasing_ValueChanged(object sender, EventArgs e)
        {
            EasingCoef = (float) numericUpDownEasing.Value / 10f;
        }

        //обработка изменения степени двойки количества сэмплов на обработку спектра
        private void numericUpDownPow2Spectrum_ValueChanged(object sender, EventArgs e)
        {
            SpectrumUseSamples = FastPowLog2Provider.FastPow2((int) numericUpDownPow2Spectrum.Value);

            SetFFTProvider();
            SetSpectrumDrawer();

            if (_currentWavFileData != null)
            {
                SetSpectrumDiagramDrawer();
            }
        }

        private void trackBarTrimFrequency_Scroll(object sender, EventArgs e)
        {
            TrimFrequency = trackBarTrimFrequency.Value;
            SetSpectrumDrawer();
            if (_currentWavFileData != null)
            {
                SetSpectrumDiagramDrawer();
            }

            labelMaxFrequency.Text = "Max Frequency: " + TrimFrequency;
        }

        private void checkBoxApplyTimeThinning_CheckedChanged(object sender, EventArgs e)
        {
            ApplyTimeThinning = checkBoxApplyTimeThinning.Checked;
            //здесь не пересоздаём массив спектра, т.к. он уже имеет нужный размер
            SetFFTProvider();
            SetSpectrumDrawer();
            if (_currentWavFileData != null)
            {
                SetSpectrumDiagramDrawer();
            }
        }

        private void hScrollBarScale_Scroll(object sender, ScrollEventArgs e)
        {
            ScaleX = hScrollBarScale.Value / 100f;
            SetWaveformProvider();
        }

        #endregion
    }
}