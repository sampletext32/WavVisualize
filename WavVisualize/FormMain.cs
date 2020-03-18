using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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

        private SpectrumDiagramDrawer _spectrumDiagramDrawer;

        public int TrimFrequency = 20000;

        //коэффициент смягчения резких скачков
        public float EasingCoef = 0.1f;

        //сколько сэмплов идёт на преобразование спектра (обязательно степень двойки)
        public int SpectrumUseSamples = 4096;

        //нажата ли сейчас мышь на волне
        public bool PressedOnWaveform;

        public bool ApplyTimeThinning = true;

        public int FramesDrawn;
        public int FramesUpdated;

        public float ScaleX = 1f;

        private TrueVolumeProvider _trueVolumeProvider;

        private Dictionary<string, object> _waveformParameters;

        private Dictionary<string, object> _realtimeSpectrumParameters;

        private Dictionary<string, object> _volumeProviderParameters;

        private Dictionary<string, object> _volumeDrawerParameters;

        private DirectBitmap _waveformBitmap;

        private DirectBitmap _spectrumBitmap;

        private DirectBitmap _volumeBitmap;

        private int _lastSamplePosition;

        #endregion

        #region Form Load

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            _waveformBitmap = new DirectBitmap(pictureBoxWaveform.Width, pictureBoxWaveform.Height);
            _spectrumBitmap = new DirectBitmap(pictureBoxRealtimeSpectrum.Width, pictureBoxRealtimeSpectrum.Height);
            _volumeBitmap = new DirectBitmap(pictureBoxVolume.Width, pictureBoxVolume.Height);

            _trueVolumeProvider = new TrueVolumeProvider();

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
            GeneralUpdate();
            GeneralRedraw();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelFPS.Text = "Drawn: " + FramesDrawn + "; Updated: " + FramesUpdated;
            FramesDrawn = 0;
            FramesUpdated = 0;
        }

        #endregion

        #region Core Setups

        private void SetPlayerProvider()
        {
            _playerProvider = new WindowsMediaPlayerProvider();
            _lastSamplePosition = 0;
            //_playerProvider.OnPlayEnd += () => { timerUpdater.Stop(); };
            //_playerProvider.OnPlayStart += () => { timerUpdater.Start(); };
        }

        private void SetVolumeProvider()
        {
            //_volumeProvider =
            //    new MaxInRegionVolumeProvider(_currentWavFileData.LeftChannel, _currentWavFileData.RightChannel,
            //        SpectrumUseSamples);
            _volumeProviderParameters = new Dictionary<string, object>();
            _volumeProviderParameters["leftChannel"] = _currentWavFileData.LeftChannel;
            _volumeProviderParameters["rightChannel"] = _currentWavFileData.RightChannel;
            _volumeProviderParameters["startSample"] = 0;
            _volumeProviderParameters["useSamples"] = SpectrumUseSamples;
            _volumeProviderParameters["type"] = 1; //MaxInRegion
        }

        private void SetWaveformProvider()
        {
            _waveformParameters = new Dictionary<string, object>();

            _waveformBitmap.Clear();

            _waveformParameters["directBitmap"] = _waveformBitmap;
            _waveformParameters["leftColor"] = (int) (0x7cfc00 | (0xFF << 24)); //LawnGreen
            _waveformParameters["rightColor"] = (int) (0xff4500 | (0xFF << 24)); //OrangeRed
            _waveformParameters["leftChannel"] = _currentWavFileData.LeftChannel;
            _waveformParameters["rightChannel"] = _currentWavFileData.RightChannel;
            _waveformParameters["samplesCount"] = _currentWavFileData.samplesCount;
            _waveformParameters["verticalScale"] = 0.9f;
            _waveformParameters["takeRate"] = 3;
            _waveformParameters["iterations"] = 2;
            _waveformParameters["splitWorkFirst"] = true;
            _waveformParameters["portions"] = 2;

            new TrueWaveformProvider().RecreateAsync(_waveformParameters);
        }

        private void SetFFTProvider()
        {
            _fftProvider = new CorrectCooleyTukeyInPlaceFFTProvider(SpectrumUseSamples, ApplyTimeThinning);
        }

        private void SetVolumeDrawer()
        {
            _volumeDrawerParameters = new Dictionary<string, object>();
            _volumeDrawerParameters["leftColor"] = (int) (0x7cfc00 | (0xFF << 24)); //LawnGreen
            _volumeDrawerParameters["rightColor"] = (int) (0xff4500 | (0xFF << 24)); //OrangeRed
            _volumeDrawerParameters["bandWidth"] = pictureBoxVolume.Width / 2;
            _volumeDrawerParameters["height"] = pictureBoxVolume.Height;
            _volumeDrawerParameters["baselineY"] = pictureBoxVolume.Height - 1;
            _volumeDrawerParameters["directBitmap"] = _volumeBitmap;
        }

        private void SetSpectrumDrawer()
        {
            _realtimeSpectrumParameters = new Dictionary<string, object>();

            _realtimeSpectrumParameters["directBitmap"] = _spectrumBitmap;
            _realtimeSpectrumParameters["baselineY"] = pictureBoxRealtimeSpectrum.Height - 1;
            _realtimeSpectrumParameters["width"] = pictureBoxRealtimeSpectrum.Width;
            _realtimeSpectrumParameters["height"] = pictureBoxRealtimeSpectrum.Height;
            _realtimeSpectrumParameters["color"] = (int) (0xff4500 | (0xFF << 24)); //OrangeRed
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

        #region Update Methods

        private void GeneralUpdate()
        {
            labelStatus.Text = _playerProvider.GetPlayState().ToString();

            float currentPosition = _playerProvider.GetElapsedSeconds();
            float duration = _playerProvider.GetDurationSeconds();

            Tuple<int, int, int> currentTime = TimeProvider.SecondsAsTime(currentPosition);
            Tuple<int, int, int> durationTime = TimeProvider.SecondsAsTime(duration);

            labelElapsed.Text =
                $@"{currentTime.Item1:00} : {currentTime.Item2:00} : {currentTime.Item3:00} / {
                    durationTime.Item1:00} : {durationTime.Item2:00} : {durationTime.Item3:00}";

            int currentSample = (int) (_currentWavFileData != null
                ? _playerProvider.GetNormalizedPosition() * _currentWavFileData.samplesCount
                : 0f);
            int deltaSamples = currentSample - _lastSamplePosition;

            if ((_playerProvider.IsPlaying() || _playerProvider.IsPaused()) && deltaSamples > 0
            ) //если сейчас воспроизводится
            {
                //если начало участка меньше чем количество сэплов - длина участка (можно вместить ещё участок)
                if (_currentWavFileData != null && currentSample < _currentWavFileData.samplesCount - deltaSamples &&
                    currentSample >= 0)
                {
                    RealtimeSpectrumUpdateCall(currentSample, deltaSamples);
                    VolumeProviderUpdateCall(currentSample, deltaSamples);
                    VolumeDrawerUpdateCall(currentSample, deltaSamples);
                }
            }

            _lastSamplePosition = currentSample;

            FramesUpdated++;
        }

        private void RealtimeSpectrumUpdateCall(int currentSample, int deltaSamples)
        {
            //рисуем спектр
            float[] spectrum = _currentWavFileData.GetSpectrumForPosition(currentSample, _fftProvider);

            int useSamples;
            if (ApplyTimeThinning)
            {
                useSamples = SpectrumUseSamples / 2 / 2;
            }
            else
            {
                useSamples = SpectrumUseSamples / 2;
            }

            //TODO: Extract Frequency Trimming
            useSamples = (int) (useSamples * TrimFrequency / 20000f);

            _spectrumBitmap.Clear();

            //float freqResolution = (float) _currentWavFileData.sampleRate / SpectrumUseSamples;

            //TODO: Add Spectrum Easing
            _realtimeSpectrumParameters["frequencies"] = spectrum;
            _realtimeSpectrumParameters["useFullCount"] = useSamples;

            TrueSpectrumDrawer.Recreate(_realtimeSpectrumParameters);
        }

        private void VolumeProviderUpdateCall(int currentSample, int deltaSamples)
        {
            _volumeProviderParameters["startSample"] = currentSample;
            _volumeProviderParameters["useSamples"] = deltaSamples;

            _trueVolumeProvider.Recreate(_volumeProviderParameters);

            //TODO: Ease volume
        }

        private void VolumeDrawerUpdateCall(int currentSample, int deltaSamples)
        {
            _volumeDrawerParameters["leftVolumeNormalized"] = _trueVolumeProvider.LastLeft;
            _volumeDrawerParameters["rightVolumeNormalized"] = _trueVolumeProvider.LastRight;

            //TODO: This place is performance critical, improve by implementing swapable buffers
            _volumeBitmap.Clear();
            TrueVolumeDrawer.Recreate(_volumeDrawerParameters);
        }

        #endregion

        #region Paint Events

        public void GeneralRedraw()
        {
            //вызываем перерисовку волны и спектра
            pictureBoxWaveform.Refresh();
            pictureBoxRealtimeSpectrum.Refresh();
            pictureBoxVolume.Refresh();
            pictureBoxSpectrumDiagram.Refresh();
            FramesDrawn++;
        }

        //перерисовка волны
        private void pictureBoxWaveform_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_waveformBitmap, 0, 0);

            //рисуем вертикальную линию текущей позиции = нормализованная позиция воспроизведения * ширину поля
            var x = (int) (_playerProvider.GetNormalizedPosition() * pictureBoxWaveform.Width);

            DrawCaret(e.Graphics, x, pictureBoxWaveform.Height, false);
        }

        //шаг отрисовки спектра
        private void pictureBoxSpectrum_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_spectrumBitmap, 0, 0);
        }

        private void pictureBoxVolume_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_volumeBitmap, 0, 0);
        }

        private void pictureBoxSpectrumDiagram_Paint(object sender, PaintEventArgs e)
        {
            _spectrumDiagramDrawer?.Draw(e.Graphics);

            //рисуем вертикальную линию текущей позиции = нормализованная позиция воспроизведения * ширину поля
            var x = (int) (_playerProvider.GetNormalizedPosition() * pictureBoxWaveform.Width);

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
            //SetSpectrumDrawer();

            if (_currentWavFileData != null)
            {
                SetSpectrumDiagramDrawer();
            }
        }

        private void trackBarTrimFrequency_Scroll(object sender, EventArgs e)
        {
            TrimFrequency = trackBarTrimFrequency.Value;
            //SetSpectrumDrawer();
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
            //SetSpectrumDrawer();
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