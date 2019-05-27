using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using WMPLib;


namespace WavVisualize
{
    public partial class FormMain : Form
    {
        //плеер
        private WindowsMediaPlayer _wmp = new WindowsMediaPlayer();

        //нормализованная позиция воспроизведения
        private float _playerPositionNormalized = 0f;

        //текущий открытый Wav файл
        private WavFileData _currentWavFileData;

        private WaveformProvider _waveformProvider;

        //текущая отображаемая громкость
        public float CurrentVolumeL;
        public float CurrentVolumeR;

        //текущий массив спектра
        public float[] CurrentSpectrum;

        //создавать ли волну последовательно
        public readonly bool CreateWaveformSequentially = true;

        //количество потоков для создания волны (-1, чтобы главный поток рисовал без зависаний)
        public readonly int ThreadsForWaveformCreation = Environment.ProcessorCount / 2 - 1;

        //частота пропуска сэмплов при создании волны
        public readonly int WaveformSkipSampleRate = 0;

        //сколько раз в секунду обновляется состояние плеера
        public readonly int UpdateRate = 60;

        //коэффициент смягчения резких скачков
        public float EasingCoef = 0.1f;

        //ширина столбиков громкости
        public readonly int BandWidth = 20;

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
        public int SpectrumUseSamples = 8192;

        //частота пропуска частот при отрисовке спектра
        public int DrawSpectrumSkipRate = 0;

        //расстояние между громкостью и спектром
        public float DistanceBetweenVolumeAndSpectrum = 20f;

        //нажати ли сейчас мышь на волне
        public bool PressedOnWaveform;

        public FormMain()
        {
            InitializeComponent();
        }

        //перерисовка волны
        private void pictureBoxPlot_Paint(object sender, PaintEventArgs e)
        {
            _waveformProvider?.Draw(e.Graphics);

            //рисуем вертикальную линию текущей позиции = нормализованная позиция воспроизведения * ширину поля
            e.Graphics.FillRectangle(Brushes.Black, _playerPositionNormalized * pictureBoxPlot.Width, 0, 1,
                pictureBoxPlot.Height);

            //рисуем каретку текущей позиции шириной 20
            e.Graphics.FillRectangle(Brushes.DarkGray, _playerPositionNormalized * pictureBoxPlot.Width - 10,
                pictureBoxPlot.Height - 5, 20, 10);
        }

        private int fps = 0;

        //шаг обновления
        private void timerUpdater_Tick(object sender, EventArgs e)
        {
            //кешируем позицию и длину трека (так чуть чуть быстрее, чем тягать dll WMP плеера)
            double currentPosition = _wmp.controls.currentPosition;
            double duration = _wmp.currentMedia.duration;

            int h = (int) currentPosition / 3600 % 3600;
            int m = (int) currentPosition / 60 % 60;
            int s = (int) currentPosition % 60;

            int h1 = (int) duration / 3600 % 3600;
            int m1 = (int) duration / 60 % 60;
            int s1 = (int) duration % 60;
            labelElapsed.Text = $@"{h:00} : {m:00} : {s:00} / {h1:00} : {m1:00} : {s1:00}";

            //высчитываем нормаль позиции воспроизведения
            _playerPositionNormalized = (float) (currentPosition / duration);

            //вызываем перерисовку волны и спектра
            pictureBoxPlot.Refresh();
            pictureBoxSpectrum.Refresh();
            fps++;
        }

        //функция возвращает максимальную громкость на каждом канале
        //на промежутке length начиная со start
        private static void GetMaxVolume(float[] left, float[] right, ref float lMax, ref float rMax, int start,
            int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (left[start + i] > lMax)
                {
                    lMax = left[start + i];
                }

                if (right[start + i] > rMax)
                {
                    rMax = right[start + i];
                }
            }
        }

        //функция возвращает среднюю громкость на каждом канале
        //на промежутке length начиная со start
        private static void GetAverageVolume(float[] left, float[] right, ref float lAver, ref float rAver, int start,
            int length)
        {
            lAver = 0f;
            rAver = 0f;
            for (int i = 0; i < length; i++)
            {
                lAver += left[start + i];
                rAver += right[start + i];
            }

            //нормализуем обе громкости
            lAver /= length;
            rAver /= length;
        }

        //шаг отрисовки спектра
        private void pictureBoxSpectrum_Paint(object sender, PaintEventArgs e)
        {
            if (_wmp.playState == WMPPlayState.wmppsPlaying) //если сейчас воспроизводится
            {
                //на каком сейчас сэмпле находимся
                int currentSample = (int) (_playerPositionNormalized * _currentWavFileData.SamplesCount);

                //длина участка сэмплов, на котором измеряем громкость
                int regionLength = _currentWavFileData.SampleRate / UpdateRate;

                //начало участка измерения (количество пройденных участков + 1) * длину одного участка
                //начинаем со следующего участка, т.к. текущий уже играет и никак не успеет отрисоваться в нужный момент
                int start = (currentSample / regionLength + 1) * regionLength;

                //если начало участка меньше чем количество сэплов - длина участка (можно вместить ещё участок)
                if (start < _currentWavFileData.SamplesCount - regionLength)
                {
                    //создаём максимальные громкости
                    float maxL = 0;
                    float maxR = 0;

                    //вычисляем громкости
                    GetMaxVolume(
                        _currentWavFileData.LeftChannel,
                        _currentWavFileData.RightChannel,
                        ref maxL, ref maxR, start, regionLength
                    );

                    //изменяем текущую нормализованую громкость на Дельту громкости * коэффициент смягчения
                    CurrentVolumeL += (maxL - CurrentVolumeL) * (1 - EasingCoef);
                    CurrentVolumeR += (maxR - CurrentVolumeR) * (1 - EasingCoef);

                    //вычисляем количество цифровых кусочков = громкость * общее количество кусочков
                    int digitalPartsL = (int) (CurrentVolumeL * DigitalBandPiecesCount);
                    int digitalPartsR = (int) (CurrentVolumeR * DigitalBandPiecesCount);

                    //рисуем линию громкости левого канала
                    e.Graphics.DrawLine(Pens.LawnGreen, 0,
                        SpectrumBaselineY - CurrentVolumeL * SpectrumHeight, BandWidth,
                        SpectrumBaselineY - CurrentVolumeL * SpectrumHeight);

                    //рисуем линию громкости правого канала
                    e.Graphics.DrawLine(Pens.OrangeRed, BandWidth,
                        SpectrumBaselineY - CurrentVolumeR * SpectrumHeight,
                        BandWidth + BandWidth,
                        SpectrumBaselineY - CurrentVolumeR * SpectrumHeight);

                    //рисуем цифровые части левой громкости
                    for (int i = 1; i < digitalPartsL + 1; i++)
                    {
                        e.Graphics.FillRectangle(Brushes.LawnGreen, 0,
                            SpectrumBaselineY - i * (DigitalPieceHeight + DistanceBetweenBands), BandWidth,
                            DigitalPieceHeight);
                    }

                    //рисуем цифровые части правой громкости
                    for (int i = 1; i < digitalPartsR + 1; i++)
                    {
                        e.Graphics.FillRectangle(Brushes.OrangeRed, BandWidth,
                            SpectrumBaselineY - i * (DigitalPieceHeight + DistanceBetweenBands), BandWidth,
                            DigitalPieceHeight);
                    }
                }

                //если начало участка меньше чем количество сэплов - сэмплов на преобразование спектра (можно вместить ещё раз рассчитать спектр)
                if (start < _currentWavFileData.SamplesCount - SpectrumUseSamples)
                {
                    //рассчитываем спектр
                    float[] newSpectrum =
                        _currentWavFileData.GetSpectrumForPosition(
                            _playerPositionNormalized,
                            SpectrumUseSamples);

                    //изменяем текущий нормализованый спектр на Дельту спектра * коэффициент смягчения
                    for (int i = 0; i < SpectrumUseSamples; i++)
                    {
                        CurrentSpectrum[i] += (newSpectrum[i] - CurrentSpectrum[i]) * (1 - EasingCoef);
                    }
                }

                //рисуем спектр
                DrawSpectrum(e.Graphics, CurrentSpectrum);
            }
        }

        //функция отвечает за отрисовку спектра
        private void DrawSpectrum(Graphics g, float[] spectrum)
        {
            //количество реально используемых сэмплов спектра (издержка быстрого преобразования Фурье)
            int useLength = SpectrumUseSamples / 2;

            int useOffset = 0;

            //количество задействованных столбиков спектра
            //минимум между количеством частот и количеством столбиков
            int useBands = Math.Min(TotalSpectrumBands, useLength);

            //ширина одного столбика
            float sbandWidth = (float) TotalSpectrumWidth / useBands;

            float frequencyResolution = (float) _currentWavFileData.SampleRate / SpectrumUseSamples;

            float[] frequencies = new float[44100];
            for (int i = 0; i < (int)(useLength / frequencyResolution); i++)
            {
                frequencies[(int) (i * frequencyResolution)] = spectrum[useOffset + i];
            }

            //множитель частоты
            float multiplier = 10f; //(float) Math.Log(SpectrumUseSamples, Math.Log(SpectrumUseSamples, 2));

            //Для того, чтобы не рисовать нулевые столбики, делаем так
            //Запоминаем текущий столбик и его максимальное значение
            //Если вдруг столбик сменился, обнуляем
            //Если встретили частоту больше уже отрисована, рисуем столбик заново

            int lastBand = -1; //последний активный столбик
            float maxInLastBand = 0f; //максимальное значение частоты в последнем столбике

            //смещаемся на 1 + DrawSpectrumSkipRate, таким образом покрывая все частоты спектра
            for (int i = 0; i < 2000; i += (1 + DrawSpectrumSkipRate))
            {
                //вычисляем номер столбика
                //нормализация номера частоты * количество_столбиков
                int band = (int) ((float) i / useLength * useBands);

                //int band = i * TotalSpectrumBands / 2000;

                //нормализованная высота столбика спектра
                //умножаем на постоянный коэффициент
                //float normalizedHeight = frequencies[i] * multiplier; //spectrum[useOffset + i] * multiplier;
                float normalizedHeight = frequencies[i] * multiplier; //spectrum[useOffset + i] * multiplier;


                //дополнительно применяем логарифмическое выравние громкости (i + 2, чтобы не получить бесконечность)
                //normalizedHeight *= (float) Math.Log(Math.Max(i - useLength / useBands, 2), 2);


                if (normalizedHeight > maxInLastBand) //если эта частота больше, чем уже отрисована
                {
                    maxInLastBand = normalizedHeight; //пересохраняем частоту
                    if (DisplayDigital) //если рисуем в цифровом виде
                    {
                        //считаем количество цифровых кусочков (нормализация высоты * общее количество кусочков)
                        int digitalParts = (int) (normalizedHeight * DigitalBandPiecesCount);

                        //рисуем цифровые части столбика
                        for (int k = 1; k < digitalParts + 1; k++)
                        {
                            g.FillRectangle(Brushes.OrangeRed,
                                BandWidth + BandWidth + DistanceBetweenVolumeAndSpectrum +
                                band * (sbandWidth + DistanceBetweenBands),
                                SpectrumBaselineY - k * (DigitalPieceHeight + DistanceBetweenBands), sbandWidth,
                                DigitalPieceHeight);
                        }
                    }
                    else //если рисуем в аналоговом виде
                    {
                        //считаем высоту (нормализация высоты * высоту спектра)
                        float analogHeight = normalizedHeight * SpectrumHeight;

                        //рисуем аналоговый столбик
                        g.FillRectangle(Brushes.OrangeRed,
                            BandWidth + BandWidth + DistanceBetweenVolumeAndSpectrum +
                            band * (sbandWidth + DistanceBetweenBands),
                            SpectrumBaselineY - analogHeight,
                            sbandWidth, analogHeight);
                    }
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }

        //когда форма открылась
        private void FormMain_Shown(object sender, EventArgs e)
        {
            //выводим коэффициенты на форму
            numericUpDown1.Value = (decimal) (EasingCoef * 10);
            numericUpDown2.Value = (decimal) (Math.Log(SpectrumUseSamples, 2));
            checkBox1.Checked = MyFFT.useCache;
            //OpenFile();
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


                await Task.Run(() =>
                {
                    //открываем файл
                    var reader = new Mp3FileReader(filename);

                    MemoryStream ms = new MemoryStream();

                    Invoke(new Action(() => { labelStatus.Text = "Converting To Wav"; }));

                    //создаём PCM поток
                    var waveStream = WaveFormatConversionStream.CreatePcmStream(reader);

                    //переписываем MP3 в Wav файл в потоке
                    WaveFileWriter.WriteWavFileToStream(ms, waveStream);

                    Invoke(new Action(() => { labelStatus.Text = "Writing Wav"; }));

                    //возвращаем поток в начало
                    ms.Seek(0, SeekOrigin.Begin);

                    Invoke(new Action(() => { labelStatus.Text = "Reading Wav"; }));

                    //читаем Wav файл
                    _currentWavFileData = WavFileData.ReadWav(ms);
                });

                Invoke(new Action(() => { labelStatus.Text = "Playing"; }));

                this.Text = opf.SafeFileName;

                //высота спектра = высоте окна
                SpectrumHeight = pictureBoxSpectrum.Height;

                //общая ширина спектра = половине ширине окна
                TotalSpectrumWidth = (int) (pictureBoxSpectrum.Width -
                                            (BandWidth + BandWidth + DistanceBetweenVolumeAndSpectrum) -
                                            DistanceBetweenBands * (TotalSpectrumBands - 1));

                //координата Y 0 спектра - высота окна
                SpectrumBaselineY = pictureBoxSpectrum.Height;

                _waveformProvider?.CancelRecreation();

                _waveformProvider = new WaveformProvider(pictureBoxPlot.Width, pictureBoxPlot.Height,
                    _currentWavFileData, CreateWaveformSequentially, ThreadsForWaveformCreation,
                    WaveformSkipSampleRate);

                Task.Run(() => _waveformProvider.StartRecreation());

                MyFFT.InitAllCache(SpectrumUseSamples);

                //просчитываем спектр на самом первом участке, это нужно для инициализации массива
                CurrentSpectrum = _currentWavFileData.GetSpectrumForPosition(0, SpectrumUseSamples);

                //создаём новый медиафайл
                _wmp.currentMedia = _wmp.newMedia(filename);
                _wmp.controls.play();

                //количество кусочков столбика = (высота окна / (высоту кусочка + расстояние между кусочками))
                DigitalBandPiecesCount = (int) (SpectrumHeight / (DigitalPieceHeight + DistanceBetweenBands));

                //изменяем интервал обновления
                timerUpdater.Interval = 1000 / UpdateRate;
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
            _wmp.controls.currentPosition = ((float) e.X / pictureBoxPlot.Width) * _wmp.currentMedia.duration;
            _wmp.controls.play();
        }

        //когда двигаем мышь по волне
        private void pictureBoxPlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (PressedOnWaveform) //если нажата кнопка
            {
                //выставляем позицию воспроизведения, как нормализацию координаты клика * длительность трека
                _wmp.controls.currentPosition = ((float) e.X / pictureBoxPlot.Width) * _wmp.currentMedia.duration;
                _wmp.controls.play();

                //вызываем перерисовку, т.к. сдвинулась позиция
                pictureBoxPlot.Refresh();
            }
        }

        //отпустили мышь над волной
        private void pictureBoxPlot_MouseUp(object sender, MouseEventArgs e)
        {
            PressedOnWaveform = false;
        }

        //обработка изменения смягчающего коэффициента
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            EasingCoef = (float) numericUpDown1.Value / 10f;
        }

        //обработка изменения степени двойки количества сэмплов на обработку спектра
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            SpectrumUseSamples = (int) Math.Pow(2, (double) numericUpDown2.Value);

            //создаём массив спектра заново, т.к. во время отрисовки массив не должен меняться
            CurrentSpectrum = new float[SpectrumUseSamples];

            MyFFT.InitAllCache(SpectrumUseSamples);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await OpenFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _waveformProvider.CancelRecreation();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            MyFFT.useCache = checkBox1.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelFPS.Text = "FPS: " + fps;
            fps = 0;
        }
    }
}