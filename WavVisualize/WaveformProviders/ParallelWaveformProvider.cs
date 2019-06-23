using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class ParallelWaveformProvider : WaveformProvider
    {
        protected int Threads;
        protected Bitmap[] Bitmaps;
        protected Brush[] LeftBrushes;
        protected Brush[] RightBrushes;

        protected int PieceWidth;

        public ParallelWaveformProvider(NestedRectangle displayRectangle, Color colorL, Color colorR,
            WavFileData fileData, float verticalScale, int threads) : base(displayRectangle, colorL, colorR,
            fileData, verticalScale)
        {
            Threads = threads;
            Bitmaps = new Bitmap[threads];
            PieceWidth = (int) Math.Ceiling(displayRectangle.Inner.Width / threads);

            LeftBrushes = new Brush[threads];
            RightBrushes = new Brush[threads];
            for (int i = 0; i < threads; i++)
            {
                Bitmaps[i] = new Bitmap(PieceWidth, (int) displayRectangle.Inner.Height);
                LeftBrushes[i] = new SolidBrush(LeftColor);
                RightBrushes[i] = new SolidBrush(RightColor);
            }
        }

        public override void Recreate()
        {
            Canceled = false;
            for (int i = 0; i < Threads; i++)
            {
                int t = i;
                Task.Run(() =>
                {
                    using (Graphics g = Graphics.FromImage(Bitmaps[t]))
                    {
                        //проходим количество_сэмплов = всего_сэмплов / количество_потоков 
                        //Сдвигаемся на 1 + skip, таким образом, даже если пропуск = 0, мы обработаем весь массив
                        for (int k = 0; k < FileData.SamplesCount / Threads; k++)
                        {
                            if (Canceled) break;
                            //Позиция по горизонтали = Нормализация_позиции_сэмпла_в_кусочке(i / (SamplesCount / threads)) * Ширину_кусочка 
                            int xPosition = (int) (k / ((float) FileData.SamplesCount / Threads) * PieceWidth);

                            //Значения PCM [номер_потока * сэмплов_на_поток + номер_текущего_сэмпла] * 
                            //(высота пополам (высота одной громкости) * масштабирование)
                            int valueL =
                                (int) (FileData.LeftChannel[t * FileData.SamplesCount / Threads + k] *
                                       (DisplayRectangle.Inner.CenterH) * VerticalScale);
                            int valueR =
                                (int) (FileData.RightChannel[t * FileData.SamplesCount / Threads + k] *
                                       (DisplayRectangle.Inner.CenterH) * VerticalScale);

                            g.FillRectangle(LeftBrushes[t], xPosition, DisplayRectangle.Inner.CenterH - valueL, 1, valueL);

                            g.FillRectangle(RightBrushes[t], xPosition, DisplayRectangle.Inner.CenterH, 1, valueR);
                        }
                    }
                });
            }
        }

        public override void Dispose()
        {
            
        }

        public override void Draw(Graphics g)
        {
            for (int i = 0; i < Threads; i++) //пробегаем все картинки
            {
                //X = нормализованному номеру потока * ширина_поля
                //Ширина = ширина_поля / количество_потоков
                g.DrawImage(Bitmaps[i],
                    (int) ((float) i / Threads * DisplayRectangle.Inner.Width), 0,
                    (int) Math.Ceiling((float) DisplayRectangle.Inner.Width / Threads),
                    DisplayRectangle.Inner.Height);
            }
        }
    }
}