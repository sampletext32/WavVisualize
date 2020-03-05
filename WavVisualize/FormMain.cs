using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;


namespace WavVisualize
{
    public partial class FormMain : Form
    {
        //плеер
        private WindowsMediaPlayerProvider _playerProvider;

        //текущий открытый Wav файл
        private WavFileData _currentWavFileData;

        private WaveformProvider _waveformProvider;

        private FFTProvider _fftProvider;

        private VolumeProvider _volumeProvider;

        private VolumeDrawer _volumeDrawer;

        private SpectrumDrawer _spectrumDrawer;

        private SpectrumDiagram _spectrumDiagram;

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

        public int Fps;

        public float ScaleX = 1f;

        private NestedRectangle _waveformRectangle;

        private void SetPlayerProvider()
        {
            _playerProvider = new WindowsMediaPlayerProvider();
        }

        private void SetVolumeProvider()
        {
            _volumeProvider =
                new MaxInRegionVolumeProvider(_currentWavFileData.LeftChannel, _currentWavFileData.RightChannel,
                    _currentWavFileData.sampleRate / UpdateRate);
        }
        
        private void SetWaveformProvider()
        {
            _waveformProvider?.Cancel();

            //_waveformProvider?.Dispose();

            _waveformRectangle = NestedRectangle.FromPictureBox(pictureBoxWaveform);
            _waveformRectangle.Outer.ScaleX(ScaleX);

            _waveformRectangle.SetInnerCenterAt(_playerProvider.GetNormalizedPosition());

            //_waveformProvider = new BasicWithIterationablePrerunWaveformProvider(
            //    _waveformRectangle, Color.LawnGreen, Color.OrangeRed, _currentWavFileData,
            //    0.9f, 40);

            _waveformProvider = new IterationableWaveformProvider(
                _waveformRectangle, Color.LawnGreen, Color.OrangeRed, _currentWavFileData,
                0.9f, 40);

            //_waveformProvider = new BasicWaveformProvider(_waveformRectangle, Color.LawnGreen, Color.OrangeRed,
            //  _currentWavFileData, 0.9f);

            _waveformProvider.Recreate();
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

        public void SetSpectrumDiagram()
        {
            _spectrumDiagram?.Cancel();
            //_spectrumDiagram = new IterationableSpectrumDiagram(SpectrumUseSamples, Rectangle.FromPictureBox(pictureBoxSpectrumDiagram), _currentWavFileData, 50);
            _spectrumDiagram = new BasicSpectrumDiagram(SpectrumUseSamples,
                Rectangle.FromPictureBox(pictureBoxSpectrumDiagram), _currentWavFileData);
            _spectrumDiagram.SetTrimmingFrequency(TrimFrequency);
            _spectrumDiagram.SetApplyTimeThinning(ApplyTimeThinning);
            _spectrumDiagram.Recreate();
        }

        public FormMain()
        {
            InitializeComponent();
            SetPlayerProvider();
            SetFFTProvider();
            SetVolumeDrawer();
            SetSpectrumDrawer();
        }

        //перерисовка волны
        private void pictureBoxWaveform_Paint(object sender, PaintEventArgs e)
        {
            _waveformProvider?.Draw(e.Graphics);

            //рисуем вертикальную линию текущей позиции = нормализованная позиция воспроизведения * ширину поля
            e.Graphics.FillRectangle(Brushes.Black, _playerProvider.GetNormalizedPosition() * pictureBoxWaveform.Width,
                0,
                1,
                pictureBoxWaveform.Height);

            //рисуем каретку текущей позиции шириной 20
            e.Graphics.FillRectangle(Brushes.DarkGray,
                _playerProvider.GetNormalizedPosition() * pictureBoxWaveform.Width - 10,
                pictureBoxWaveform.Height - 5, 20, 5);

            ////рисуем вертикальную линию текущей позиции = нормализованная позиция воспроизведения * ширину поля
            //e.Graphics.FillRectangle(Brushes.Black, pictureBoxWaveform.Width / 2,
            //    0,
            //    1,
            //    pictureBoxWaveform.Height);

            ////рисуем каретку текущей позиции шириной 20
            //e.Graphics.FillRectangle(Brushes.DarkGray, pictureBoxWaveform.Width / 2 - 10,
            //    pictureBoxWaveform.Height - 5, 20, 5);
        }


        //шаг обновления
        private void timerUpdater_Tick(object sender, EventArgs e)
        {
            labelStatus.Text = _playerProvider.GetPlayState().ToString();

            //if (Fps % 2 == 0)
            {
                _waveformRectangle.SetInnerCenterAt(_playerProvider.GetNormalizedPosition());
                //_waveformProvider.Recreate();
            }

            float currentPosition = _playerProvider.GetElapsedSeconds();
            float duration = _playerProvider.GetDurationSeconds();

            Tuple<int, int, int> currentTime = TimeProvider.SecondsAsTime(currentPosition);
            Tuple<int, int, int> durationTime = TimeProvider.SecondsAsTime(duration);

            labelElapsed.Text =
                $@"{currentTime.Item1:00} : {currentTime.Item2:00} : {currentTime.Item3:00} / {
                        durationTime.Item1
                    :00} : {durationTime.Item2:00} : {durationTime.Item3:00}";

            //вызываем перерисовку волны и спектра
            pictureBoxWaveform.Refresh();
            pictureBoxRealtimeSpectrum.Refresh();
            pictureBoxVolume.Refresh();
            pictureBoxSpectrumDiagram.Refresh();
            Fps++;
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

                    ////рисуем линию громкости левого канала
                    //e.Graphics.DrawLine(Pens.LawnGreen, 0,
                    //    SpectrumBaselineY - CurrentVolumeL * SpectrumHeight, VolumeBandWidth,
                    //    SpectrumBaselineY - CurrentVolumeL * SpectrumHeight);
                    //
                    ////рисуем линию громкости правого канала
                    //e.Graphics.DrawLine(Pens.OrangeRed, VolumeBandWidth,
                    //    SpectrumBaselineY - CurrentVolumeR * SpectrumHeight,
                    //    VolumeBandWidth + VolumeBandWidth,
                    //    SpectrumBaselineY - CurrentVolumeR * SpectrumHeight);
                }
            }
        }

        private void pictureBoxSpectrumDiagram_Paint(object sender, PaintEventArgs e)
        {
            _spectrumDiagram?.Draw(e.Graphics);

            //рисуем вертикальную линию текущей позиции = нормализованная позиция воспроизведения * ширину поля
            e.Graphics.FillRectangle(Brushes.Black, _playerProvider.GetNormalizedPosition() * pictureBoxWaveform.Width,
                0,
                1,
                pictureBoxSpectrumDiagram.Height);

            //рисуем каретку текущей позиции шириной 20
            e.Graphics.FillRectangle(Brushes.DarkGray,
                _playerProvider.GetNormalizedPosition() * pictureBoxWaveform.Width - 10, 0, 20, 2);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }

        //когда форма открылась
        private void FormMain_Shown(object sender, EventArgs e)
        {
            //выводим коэффициенты на форму
            numericUpDownEasing.Value = (int) (EasingCoef * 10);

            numericUpDownPow2Spectrum.Value = FastPowLog2Provider.FastLog2(SpectrumUseSamples);

            trackBarTrimFrequency.Value = TrimFrequency;

            checkBoxApplyTimeThinning.Checked = ApplyTimeThinning;

            labelMaxFrequency.Text = "Max Frequency: " + TrimFrequency;
        }

        async Task OpenFile()
        {
            OpenFileDialog opf = new OpenFileDialog();
            //opf.Filter = "Файлы WAV (*.wav)|*.wav";
            opf.Filter = "Файлы MP3 (*.mp3)|*.mp3";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                string filename = opf.FileName;

                labelStatus.Text = "Opening";
                Application.DoEvents();

                WavFileData _data = null;

                await Task.Run(() =>
                {
                    //открываем файл
                    var reader = new Mp3FileReader(filename);
                    
                    Invoke(new Action(() => { labelStatus.Text = "Converting To Wav"; }));

                    //создаём PCM поток
                    var waveStream = WaveFormatConversionStream.CreatePcmStream(reader);
                    
                    MemoryStream ms = new MemoryStream();

                    //переписываем MP3 в Wav файл в потоке
                    WaveFileWriter.WriteWavFileToStream(ms, waveStream);

                    Invoke(new Action(() => { labelStatus.Text = "Writing Wav"; }));

                    //возвращаем поток в начало
                    ms.Seek(0, SeekOrigin.Begin);

                    Invoke(new Action(() => { labelStatus.Text = "Reading Wav"; }));

                    //читаем Wav файл
                    _data = new WavFileData(ms.ToArray());

                    ms.Dispose();
                });

                Invoke(new Action(() => { labelStatus.Text = "Playing"; }));

                this.Text = opf.SafeFileName;

                _currentWavFileData = _data;

                SetWaveformProvider();

                SetVolumeProvider();

                //создаём новый медиафайл
                _playerProvider.SetFile(filename);

                //SetSpectrumDrawer();
                SetSpectrumDiagram();


                //изменяем интервал обновления
                timerUpdater.Interval = 1;
                timerUpdater.Start(); //запускаем обновление

                //по невероятной причине, из-за открытия диалогового окна, форма находится не в фокусе
                //поэтому выводим форму на первый план
                this.BringToFront();
            }
            else //если файл не выбран, закрыть приложение
            {
            }
        }

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
                SetSpectrumDiagram();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelFPS.Text = "FPS: " + Fps;
            Fps = 0;
        }

        private async void buttonOpenFile_Click(object sender, EventArgs e)
        {
            await OpenFile();
        }

        private void trackBarTrimFrequency_Scroll(object sender, EventArgs e)
        {
            TrimFrequency = trackBarTrimFrequency.Value;
            SetSpectrumDrawer();
            if (_currentWavFileData != null)
            {
                SetSpectrumDiagram();
            }

            labelMaxFrequency.Text = "Max Frequency: " + TrimFrequency;
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

        private void checkBoxApplyTimeThinning_CheckedChanged(object sender, EventArgs e)
        {
            ApplyTimeThinning = !ApplyTimeThinning;
            //здесь не пересоздаём массив спектра, т.к. он уже имеет нужный размер
            SetFFTProvider();
            SetSpectrumDrawer();
            if (_currentWavFileData != null)
            {
                SetSpectrumDiagram();
            }
        }

        private void hScrollBarScale_Scroll(object sender, ScrollEventArgs e)
        {
            ScaleX = hScrollBarScale.Value / 100f;
            SetWaveformProvider();
        }
    }
}