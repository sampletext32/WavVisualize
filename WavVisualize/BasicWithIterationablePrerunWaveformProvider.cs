using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class BasicWithIterationablePrerunWaveformProvider : WaveformProvider
    {
        protected Bitmap CacheBitmap;
        protected Brush LeftBrush;
        protected Brush RightBrush;
        protected int PrerunIterations;

        public BasicWithIterationablePrerunWaveformProvider(float left, float right, float top, float bottom,
            Color colorL,
            Color colorR, WavFileData fileData, float verticalScale, int prerunIterations) : base(left, right, top,
            bottom,
            colorL, colorR,
            fileData, verticalScale)
        {
            CacheBitmap = new Bitmap((int) Width, (int) Height);
            LeftBrush = new SolidBrush(LeftColor);
            RightBrush = new SolidBrush(RightColor);

            PrerunIterations = prerunIterations;
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(CacheBitmap, 0, 0, Width, Height);
        }

        public override void Recreate()
        {
            Task.Run(() =>
            {
                Graphics g = Graphics.FromImage(CacheBitmap);

                for (int k = 0; k < PrerunIterations; k++)
                {
                    for (int i = k; i < FileData.SamplesCount; i += FileData.SampleRate / PrerunIterations)
                    {
                        if (Canceled)
                        {
                            break;
                        }

                        int xPosition = (int) (i / (float) FileData.SamplesCount * Width);

                        int valueL =
                            (int) (FileData.LeftChannel[i] * (Height / 2) * VerticalScale);
                        int valueR =
                            (int) (FileData.RightChannel[i] * (Height / 2) * VerticalScale);

                        lock (g)
                        {
                            g.FillRectangle(LeftBrush, xPosition, Height / 2 - valueL, 1, valueL);

                            g.FillRectangle(RightBrush, xPosition, Height / 2, 1, valueR);
                        }
                    }
                }

                for (int k = 0; k < FileData.SamplesCount; k++)
                {
                    if (Canceled) break;
                    int xPosition = (int) (k / (float) FileData.SamplesCount * Width);

                    int valueL =
                        (int) (FileData.LeftChannel[k] * (Height / 2) * VerticalScale);
                    int valueR =
                        (int) (FileData.RightChannel[k] * (Height / 2) * VerticalScale);

                    g.FillRectangle(LeftBrush, xPosition, Height / 2 - valueL, 1, valueL);

                    g.FillRectangle(RightBrush, xPosition, Height / 2, 1, valueR);
                }

                g.Dispose();
            });
        }
    }
}