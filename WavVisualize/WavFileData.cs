using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WavVisualize
{
    //Класс отвечающий за работу с Wav файлом
    class WavFileData
    {
        #region Параметры Wav файла

        public int ChunkId;
        public int FileSize;
        public int RiffType;
        public int FmtId;
        public int FmtSize;
        public int FmtExtraSize;
        public int FmtCode;
        public int Channels;
        public int SampleRate;
        public int ByteRate;
        public int FmtBlockAlign;
        public int BitDepth;
        public int DataId;
        public int BytesLength;
        public int BytesForSample;
        public int SamplesCount;

        #endregion

        #region Используемые для вычислений данные

        public float[] LeftChannel;
        public float[] RightChannel;
        public Bitmap[] WaveformBitmaps;

        #endregion

        //Функция отвечает за последовательную перерисовку волны
        //Визуализация:
        //----------------
        //1-------1-------
        //12------12------
        //123-----123-----
        //1234----1234----
        //12345---12345---
        //123456--123456--
        //1234567-1234567-
        //1234567812345678
        public void RecreateWaveformBitmapSequentially(int width, int height, int skip = 0, int threads = 1)
        {
            #region Создание цветов и кистей

            //Вся эта огромная хрень с массивами кистей нужна потом, что GDI(Graphics) не умеет рисовать одной кистью в нескольких потоках сразу

            Color greenColor = Color.FromArgb(255 / 10, Color.LawnGreen);
            Brush[] greenBrushes = new Brush[threads];

            Color redColor = Color.FromArgb(255 / 10, Color.OrangeRed);
            Brush[] redBrushes = new Brush[threads];

            #endregion

            float pieceWidth =
                (float) Math.Ceiling((float) width / threads); //ширина_картинки = общая_ширина / количество_потоков

            float yScale = 0.8f; //масштабирование по высоте
            int yCenter = height / 2; //центр отрисовки

            WaveformBitmaps = new Bitmap[threads]; //создаём массив картинок

            for (int thread = 0; thread < threads; thread++)
            {
                WaveformBitmaps[thread] = new Bitmap((int) pieceWidth, height);

                //Создаём кисти
                greenBrushes[thread] = new SolidBrush(greenColor);
                redBrushes[thread] = new SolidBrush(redColor);

                int t = thread; //копируем номер потока (это нужно для безопасного обращения к многопоточным индексам)
                //запускаем задачу по отрисовке
                Task.Run(() =>
                {
                    using (Graphics g = Graphics.FromImage(WaveformBitmaps[t]))
                    {
                        if (LeftChannel != null && RightChannel != null)
                        {
                            //проходим количество_сэмплов = всего_сэмплов / количество_потоков 
                            //Сдвигаемся на 1 + skip, таким образом, даже если пропуск = 0, мы обработаем весь массив
                            for (int i = 0; i < ((float) SamplesCount / threads); i += (1 + skip))
                            {
                                //Позиция по горизонтали = Нормализация_позиции_сэмпла_в_кусочке(i / (SamplesCount / threads)) * Ширину_кусочка 
                                int xPosition = (int) (i / ((float) SamplesCount / threads) * pieceWidth);

                                //Значения PCM [номер_потока * сэмплов_на_поток + номер_текущего_сэмпла] * 
                                //(высота пополам (высота одной громкости) * масштабирование)
                                int valueL =
                                    (int) (LeftChannel[t * SamplesCount / threads + i] * (height / 2f) * yScale);
                                int valueR =
                                    (int) (RightChannel[t * SamplesCount / threads + i] * (height / 2f) * yScale);

                                g.FillRectangle(greenBrushes[t], xPosition, yCenter - valueL, 1, valueL);

                                g.FillRectangle(redBrushes[t], xPosition, yCenter, 1, valueR);
                            }
                        }
                    }
                }).ContinueWith((task) =>
                {
                    //завершить задачи по отрисовке удалением кистей
                    greenBrushes[t].Dispose();
                    redBrushes[t].Dispose();
                });
            }
        }

        //Функция отвечает за параллельную перерисовку волны
        //Визуализация:
        //--------
        //1-------
        //21------
        //321-----
        //4321----
        //44321---
        //444321--
        //4444321-
        //44444321
        public void RecreateWaveformBitmapParallel(int width, int height, int skip = 0, int threads = 1)
        {
            #region Создание цветов и кистей

            //Вся эта огромная хрень с массивами кистей нужна потом, что GDI(Graphics) не умеет рисовать одной кистью в нескольких потоках сразу

            Color greenColor = Color.FromArgb(255 / 10, Color.LawnGreen);
            Brush[] greenBrushes = new Brush[threads];

            Color redColor = Color.FromArgb(255 / 10, Color.OrangeRed);
            Brush[] redBrushes = new Brush[threads];

            #endregion


            float yScale = 0.8f; //масштабирование по высоте
            float yCenter = height / 2f; //центр отрисовки

            WaveformBitmaps = new Bitmap[threads]; //создаём массив картинок

            for (int thread = 0; thread < threads; thread++)
            {
                WaveformBitmaps[thread] = new Bitmap(width, height);

                //Создаём кисти
                greenBrushes[thread] = new SolidBrush(greenColor);
                redBrushes[thread] = new SolidBrush(redColor);

                int t = thread; //копируем номер потока (это нужно для безопасного обращения к многопоточным индексам)
                //запускаем задачу по отрисовке
                Task.Run(() =>
                {
                    using (Graphics g = Graphics.FromImage(WaveformBitmaps[t]))
                    {
                        if (LeftChannel != null && RightChannel != null)
                        {
                            //бежим по всем сэмплам начиная с (номер_потока)
                            //Сдвигаемся на 1 + skip, таким образом, даже если пропуск = 0, мы обработаем весь массив
                            //умножаем, чтобы поток не наступил на чужие сэмплы, а пропустил собственные
                            for (int i = t; i < SamplesCount; i += threads * (1 + skip))
                            {
                                //позиция по горизонтали = нормализация_сэмпла * ширину_картинки
                                float xPosition = (float) i / SamplesCount * width;

                                //Значения PCM [номер_текущего_сэмпла] * 
                                //(высота пополам (высота одной громкости) * масштабирование)
                                float valueL = LeftChannel[i] * (height / 2f) * yScale;
                                float valueR = RightChannel[i] * (height / 2f) * yScale;

                                g.FillRectangle(greenBrushes[t], xPosition, yCenter - valueL, 1, valueL);

                                g.FillRectangle(redBrushes[t], xPosition, yCenter, 1, valueR);
                            }
                        }
                    }
                }).ContinueWith((task) =>
                {
                    //завершить задачи по отрисовке удалением кистей
                    greenBrushes[t].Dispose();
                    redBrushes[t].Dispose();
                });
            }
        }

        //функция рассчитывает спектр для заданного количества сэмплов начиная с нормализованной позиции
        public float[] GetSpectrumForPosition(float position, int spectrumUseSamples)
        {
            float[] spectrum = MyFFT.FFT(LeftChannel, (int) (SamplesCount * position), spectrumUseSamples);
            return spectrum;
        }

        //функция читает Wav файл из потока
        public static WavFileData ReadWav(Stream stream)
        {
            WavFileData wavFileData = new WavFileData();
            try
            {
                using (stream)
                {
                    BinaryReader reader = new BinaryReader(stream);

                    wavFileData.ChunkId = reader.ReadInt32();
                    wavFileData.FileSize = reader.ReadInt32();
                    wavFileData.RiffType = reader.ReadInt32();

                    wavFileData.FmtId = reader.ReadInt32();
                    wavFileData.FmtSize = reader.ReadInt32();
                    wavFileData.FmtCode = reader.ReadInt16();
                    wavFileData.Channels = reader.ReadInt16();
                    wavFileData.SampleRate = reader.ReadInt32();
                    wavFileData.ByteRate = reader.ReadInt32();
                    wavFileData.FmtBlockAlign = reader.ReadInt16();
                    wavFileData.BitDepth = reader.ReadInt16();

                    if (wavFileData.FmtSize == 18)
                    {
                        wavFileData.FmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(wavFileData.FmtExtraSize);
                    }

                    wavFileData.DataId = reader.ReadInt32();
                    wavFileData.BytesLength = reader.ReadInt32();

                    byte[] data = reader.ReadBytes(wavFileData.BytesLength);

                    wavFileData.BytesForSample =
                        wavFileData.BitDepth / 8; //байт на сэмпл - (глубина кодирования в битах) / 8(бит в байте)

                    //количество сэмплов =  общее_количество_байт   / байт_на_сэмпл/ количество_каналов
                    //                      Сумма                   / Скорость      / Параллельность
                    wavFileData.SamplesCount =
                        wavFileData.BytesLength / wavFileData.BytesForSample / wavFileData.Channels;

                    float[] asFloat;
                    switch (wavFileData.BitDepth)
                    {
                        case 64:
                            //количество значений = количество_сэмплов * количество_каналов
                            var asDouble = new double[wavFileData.SamplesCount * wavFileData.Channels];
                            Buffer.BlockCopy(data, 0, asDouble, 0, wavFileData.BytesLength);
                            asFloat = Array.ConvertAll(asDouble, e => (float) e);
                            break;
                        case 32:
                            //количество значений = количество_сэмплов * количество_каналов
                            asFloat = new float[wavFileData.SamplesCount * wavFileData.Channels];
                            Buffer.BlockCopy(data, 0, asFloat, 0, wavFileData.BytesLength);
                            break;
                        case 16:
                            //количество значений = количество_сэмплов * количество_каналов
                            var asInt16 = new short[wavFileData.SamplesCount * wavFileData.Channels];
                            Buffer.BlockCopy(data, 0, asInt16, 0, wavFileData.BytesLength);
                            asFloat = Array.ConvertAll(asInt16,
                                e => e / (float) short.MaxValue); //нормализуем, деля на максимальное значение
                            break;
                        default: throw new FormatException("Unknown BitDepth");
                    }

                    switch (wavFileData.Channels)
                    {
                        case 1:
                            //если записано МОНО, оба канала = исходному
                            wavFileData.LeftChannel = asFloat;
                            wavFileData.RightChannel = asFloat;
                            break;
                        case 2:
                            //если записано СТЕРЕО
                            //создаём 2 массива на левый и правый канал с количеством сэмплов
                            wavFileData.LeftChannel = new float[wavFileData.SamplesCount];
                            wavFileData.RightChannel = new float[wavFileData.SamplesCount];
                            for (int i = 0, s = 0; i < wavFileData.SamplesCount; i++)
                            {
                                //записываем сэмплы
                                wavFileData.LeftChannel[i] = asFloat[s++];
                                wavFileData.RightChannel[i] = asFloat[s++];
                            }

                            break;
                        default: throw new FormatException("Unknown Channels");
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return wavFileData;
        }

        //функция читает Wav файл из массива байт
        public static WavFileData ReadWav(byte[] stream)
        {
            WavFileData wavFileData = ReadWav(new MemoryStream(stream));

            return wavFileData;
        }

        //функция читает Wav файл, открывая файл по переданному пути
        public static WavFileData ReadWav(string filename)
        {
            WavFileData wavFileData = ReadWav(File.OpenRead(filename));

            return wavFileData;
        }
    }
}