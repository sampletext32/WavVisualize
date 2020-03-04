using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class IterationableWaveformProvider : WaveformProvider
    {
        protected DirectBitmap CacheBitmap;
        protected Brush LeftBrush;
        protected Brush RightBrush;
        protected int Iterations;

        public IterationableWaveformProvider(NestedRectangle displayRectangle, Color colorL,
            Color colorR, WavFileData fileData, float verticalScale, int iterations) : base(displayRectangle,
            colorL, colorR,
            fileData, verticalScale)
        {
            CacheBitmap = new DirectBitmap((int) displayRectangle.Inner.Width, (int) displayRectangle.Inner.Height);
            LeftBrush = new SolidBrush(LeftColor);
            RightBrush = new SolidBrush(RightColor);

            Iterations = iterations;
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(CacheBitmap.Bitmap, 0, 0, DisplayRectangle.Inner.Width, DisplayRectangle.Inner.Height);
        }

        public override void Recreate()
        {
            Task.Run(() =>
            {
                using (Graphics g = Graphics.FromImage(CacheBitmap.Bitmap))
                {
                    float verticalHalf = DisplayRectangle.Inner.CenterH;
                    float verticalQuarter = verticalHalf / 2;
                    float verticalThreeQuarters = verticalHalf * 3f / 2f;

                    Parallel.For(0, Iterations, (k, loopState) =>
                    {
                        for (int i = k; i < FileData.samplesCount; i += Iterations)
                        {
                            if (Canceled)
                            {
                                loopState.Break();
                                return;
                            }

                            int xPosition =
                                (int) DisplayRectangle.Outer.NormalizedWidth(i / (float) FileData.samplesCount);

                            int valueL =
                                (int) (FileData.LeftChannel[i] * verticalQuarter * VerticalScale);


                            int valueR =
                                (int) (FileData.RightChannel[i] * verticalQuarter * VerticalScale);

                            if (valueL < 0)
                            {
                                for (float y = verticalQuarter; y < verticalQuarter - valueL; y++)
                                {
                                    CacheBitmap.SetPixel(xPosition, (int) y, LeftColor.ToArgb());
                                }
                            }
                            else
                            {
                                for (float y = verticalQuarter - valueL; y < verticalQuarter; y++)
                                {
                                    CacheBitmap.SetPixel(xPosition, (int) y, LeftColor.ToArgb());
                                }
                            }

                            if (valueR < 0)
                            {
                                for (float y = verticalThreeQuarters; y < verticalThreeQuarters - valueR; y++)
                                {
                                    CacheBitmap.SetPixel(xPosition, (int) y, RightColor.ToArgb());
                                }
                            }
                            else
                            {
                                for (float y = verticalThreeQuarters - valueR; y < verticalThreeQuarters; y++)
                                {
                                    CacheBitmap.SetPixel(xPosition, (int) y, RightColor.ToArgb());
                                }
                            }
                        }
                    });
                }
            });
        }

        public override void Dispose()
        {
        }

        //private void RecreateV2()
        //{
        //    Task.Run(() =>
        //    {
        //        using (Graphics g = Graphics.FromImage(CacheBitmap))
        //        {
        //            Parallel.For(0, FileData.samplesCount, (k, loopState) =>
        //            {
        //                if (Canceled)
        //                {
        //                    loopState.Break();
        //                    return;
        //                }

        //                int xPosition = (int) DisplayRectangle.NormalizedWidth(k / (float) FileData.samplesCount);

        //                int valueL =
        //                    (int) (FileData.LeftChannel[k] * (DisplayRectangle.CenterH) * VerticalScale);
        //                int valueR =
        //                    (int) (FileData.RightChannel[k] * (DisplayRectangle.CenterH) * VerticalScale);

        //                lock (g)
        //                {
        //                    g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.CenterH - valueL, 1, valueL);

        //                    g.FillRectangle(RightBrush, xPosition, DisplayRectangle.CenterH, 1, valueR);
        //                }
        //            });
        //        }
        //    });
        //}
    }
}