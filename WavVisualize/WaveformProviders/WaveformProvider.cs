using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WavVisualize
{
    class WaveformProvider
    {
        private int _width;
        private int _height;
        private WavFileData _wavFileData;
        private Bitmap[] _waveformBitmaps;

        private Thread[] _threads;

        private bool _createWaveformSequentially;
        private int _threadsForWaveformCreation;
        private int _waveformSkipSampleRate;

        private static Color _greenColor = Color.FromArgb(255 / 10, Color.LawnGreen);
        private Brush[] _greenBrushes;

        private static Color _redColor = Color.FromArgb(255 / 10, Color.OrangeRed);
        private Brush[] _redBrushes;

        private bool _recreating;

        public void Draw(Graphics g)
        {
            if (_waveformBitmaps != null) //если есть картинки волны
            {
                if (_createWaveformSequentially) //если рисуем последовательные куски
                {
                    for (int i = 0; i < _threadsForWaveformCreation; i++) //пробегаем все картинки
                    {
                        //X = нормализованному номеру потока * ширина_поля
                        //Ширина = ширина_поля / количество_потоков
                        g.DrawImage(
                            _waveformBitmaps[i],
                            (int) ((float) i / _threadsForWaveformCreation * _width), 0,
                            (int) Math.Ceiling((float) _width / _threadsForWaveformCreation),
                            _height);
                    }
                }
                else //если рисуем параллельно
                {
                    for (int i = 0; i < _threadsForWaveformCreation; i++)
                    {
                        //рисуем волну на всей области
                        g.DrawImage(_waveformBitmaps[i],
                            0, 0,
                            _width,
                            _height);
                    }
                }
            }
        }

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
        private void RecreateWaveformBitmapSequentially()
        {
            float pieceWidth =
                (float) Math.Ceiling((float) _width /
                                     _threadsForWaveformCreation); //ширина_картинки = общая_ширина / количество_потоков

            float yScale = 0.8f; //масштабирование по высоте
            int yCenter = _height / 2; //центр отрисовки

            _waveformBitmaps = new Bitmap[_threadsForWaveformCreation]; //создаём массив картинок

            for (int threadId = 0; threadId < _threadsForWaveformCreation; threadId++)
            {
                _waveformBitmaps[threadId] = new Bitmap((int) pieceWidth, _height);

                int t = threadId; //копируем номер потока (это нужно для безопасного обращения к многопоточным индексам)
                //запускаем задачу по отрисовке

                Thread thread = new Thread(() =>
                {
                    using (Graphics g = Graphics.FromImage(_waveformBitmaps[t]))
                    {
                        try
                        {
                            if (_wavFileData.LeftChannel != null && _wavFileData.RightChannel != null)
                            {
                                //проходим количество_сэмплов = всего_сэмплов / количество_потоков 
                                //Сдвигаемся на 1 + skip, таким образом, даже если пропуск = 0, мы обработаем весь массив
                                for (int i = 0;
                                    i < ((float)_wavFileData.SamplesCount / _threadsForWaveformCreation);
                                    i += (1 + _waveformSkipSampleRate))
                                {
                                    //Позиция по горизонтали = Нормализация_позиции_сэмпла_в_кусочке(i / (SamplesCount / threads)) * Ширину_кусочка 
                                    int xPosition =
                                        (int)(i / ((float)_wavFileData.SamplesCount / _threadsForWaveformCreation) *
                                               pieceWidth);

                                    //Значения PCM [номер_потока * сэмплов_на_поток + номер_текущего_сэмпла] * 
                                    //(высота пополам (высота одной громкости) * масштабирование)
                                    int valueL =
                                        (int)(_wavFileData.LeftChannel[
                                                   t * _wavFileData.SamplesCount / _threadsForWaveformCreation + i] *
                                               (_height / 2f) * yScale);
                                    int valueR =
                                        (int)(_wavFileData.RightChannel[
                                                   t * _wavFileData.SamplesCount / _threadsForWaveformCreation + i] *
                                               (_height / 2f) * yScale);

                                    g.FillRectangle(_greenBrushes[t], xPosition, yCenter - valueL, 1, valueL);

                                    g.FillRectangle(_redBrushes[t], xPosition, yCenter, 1, valueR);
                                }
                            }
                        }
                        catch (ThreadAbortException abortException)
                        {
                            
                        }
                    }
                    NotifyThreadFinished();
                });
                _threads[t] = thread;
            }

            for (int threadId = 0; threadId < _threadsForWaveformCreation; threadId++)
            {
                _threads[threadId].Start();
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
        private void RecreateWaveformBitmapParallel()
        {
            float yScale = 0.8f; //масштабирование по высоте
            float yCenter = _height / 2f; //центр отрисовки

            for (int threadId = 0; threadId < _threadsForWaveformCreation; threadId++)
            {
                _waveformBitmaps[threadId] = new Bitmap(_width, _height);

                int t = threadId; //копируем номер потока (это нужно для безопасного обращения к многопоточным индексам)
                //запускаем задачу по отрисовке

                Thread thread = new Thread(() =>
                {
                    using (Graphics g = Graphics.FromImage(_waveformBitmaps[t]))
                    {
                        try
                        {
                            if (_wavFileData.LeftChannel != null && _wavFileData.RightChannel != null)
                            {
                                //бежим по всем сэмплам начиная с (номер_потока)
                                //Сдвигаемся на 1 + skip, таким образом, даже если пропуск = 0, мы обработаем весь массив
                                //умножаем, чтобы поток не наступил на чужие сэмплы, а пропустил собственные
                                for (int i = t;
                                    i < _wavFileData.SamplesCount;
                                    i += _threadsForWaveformCreation * (1 + _waveformSkipSampleRate))
                                {
                                    //позиция по горизонтали = нормализация_сэмпла * ширину_картинки
                                    float xPosition = (float)i / _wavFileData.SamplesCount * _width;

                                    //Значения PCM [номер_текущего_сэмпла] * 
                                    //(высота пополам (высота одной громкости) * масштабирование)
                                    float valueL = _wavFileData.LeftChannel[i] * (_height / 2f) * yScale;
                                    float valueR = _wavFileData.RightChannel[i] * (_height / 2f) * yScale;

                                    g.FillRectangle(_greenBrushes[t], xPosition, yCenter - valueL, 1, valueL);

                                    g.FillRectangle(_redBrushes[t], xPosition, yCenter, 1, valueR);
                                }
                            }
                        }
                        catch (ThreadAbortException abortException)
                        {
                            
                        }
                    }
                    NotifyThreadFinished();
                });
                _threads[t] = thread;
            }

            for (int threadId = 0; threadId < _threadsForWaveformCreation; threadId++)
            {
                _threads[threadId].Start();
            }
        }

        private void NotifyThreadFinished()
        {
            bool recreating = false;
            for (int i = 0; i < _threadsForWaveformCreation; i++)
            {
                if (_threads[i].ThreadState == ThreadState.Running)
                {
                    recreating = true;
                }
            }

            _recreating = recreating;
        }

        public void StartRecreation()
        {
            if (_recreating)
            {
                CancelRecreation();
            }

            _recreating = true;

            if (_createWaveformSequentially)
            {
                RecreateWaveformBitmapSequentially();
            }
            else
            {
                RecreateWaveformBitmapParallel();
            }
        }

        public void CancelRecreation()
        {
            for (int i = 0; i < _threadsForWaveformCreation; i++)
            {
                _threads[i].Abort();
            }

            _recreating = false;
        }

        private void InitiateBrushes()
        {
            for (int thread = 0; thread < _threadsForWaveformCreation; thread++)
            {
                _greenBrushes[thread] = new SolidBrush(_greenColor);
                _redBrushes[thread] = new SolidBrush(_redColor);
            }
        }

        public WaveformProvider(int width, int height, WavFileData wavFileData, bool createWaveformSequentially,
            int threadsForWaveformCreation, int waveformSkipSampleRate)
        {
            _width = width;
            _height = height;
            _wavFileData = wavFileData;
            _createWaveformSequentially = createWaveformSequentially;
            _threadsForWaveformCreation = threadsForWaveformCreation;
            _waveformSkipSampleRate = waveformSkipSampleRate;

            _waveformBitmaps = new Bitmap[threadsForWaveformCreation]; //создаём массив картинок

            _threads = new Thread[threadsForWaveformCreation];

            _greenBrushes = new Brush[threadsForWaveformCreation];

            _redBrushes = new Brush[threadsForWaveformCreation];

            InitiateBrushes();
            _recreating = false;
        }
    }
}