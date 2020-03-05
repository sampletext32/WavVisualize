using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class BasicWithIterationablePrerunWaveformProvider : WaveformProvider
    {
        protected DirectBitmap CacheBitmap;
        protected DirectBitmap ReadyBitmap;
        protected Brush LeftBrush;
        protected Brush RightBrush;
        protected int PrerunIterations;

        public BasicWithIterationablePrerunWaveformProvider(NestedRectangle displayRectangle,
            Color colorL,
            Color colorR, WavFileData fileData, float verticalScale, int prerunIterations, bool isWaveformScannable) :
            base(displayRectangle,
                colorL, colorR,
                fileData, verticalScale, isWaveformScannable)
        {
            CacheBitmap = new DirectBitmap((int) displayRectangle.Inner.Width, (int) displayRectangle.Inner.Height);
            ReadyBitmap = new DirectBitmap((int) displayRectangle.Inner.Width, (int) displayRectangle.Inner.Height);
            LeftBrush = new SolidBrush(LeftColor);
            RightBrush = new SolidBrush(RightColor);

            PrerunIterations = prerunIterations;
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(ReadyBitmap.Bitmap, 0, 0, DisplayRectangle.Inner.Width, DisplayRectangle.Inner.Height);
        }

        public override void Recreate()
        {
            var leftArgb = LeftColor.ToArgb();
            var rightArgb = RightColor.ToArgb();
            Task.Run(() =>
            {
                float verticalHalf = DisplayRectangle.Inner.CenterH;
                float verticalQuarter = verticalHalf / 2;
                float verticalThreeQuarters = verticalHalf * 3f / 2f;
                while (!Canceled)
                {
                    //Graphics g = Graphics.FromImage(CacheBitmap.Bitmap);
                    //g.Clear(Color.White);
                    CacheBitmap.Clear();

                    int startSample = (int) Math.Max(DisplayRectangle.InnerLeftNormalized() * FileData.samplesCount, 0);
                    int endSample = (int) Math.Min(DisplayRectangle.InnerRightNormalized() * FileData.samplesCount,
                        FileData.samplesCount);
                    int deltaSamples =
                        (int) ((float) FileData.sampleRate / PrerunIterations * DisplayRectangle.Relation());
                    startSample += deltaSamples - startSample % deltaSamples;
                    for (int k = 0; k < PrerunIterations; k++)
                    {
                        int lastX = 0;
                        int maxValL = 0;
                        int maxValR = 0;
                        for (int currentSample = startSample + k;
                            currentSample < endSample;
                            currentSample += deltaSamples)
                        {
                            if (Canceled)
                            {
                                break;
                            }

                            int xPosition =
                                (int) (DisplayRectangle.Outer.NormalizedWidth(
                                           currentSample / (float) FileData.samplesCount) -
                                       DisplayRectangle.DeltaLeft());

                            int valueL =
                                (int) (FileData.LeftChannel[currentSample] * verticalQuarter * VerticalScale);


                            int valueR =
                                (int) (FileData.RightChannel[currentSample] * verticalQuarter * VerticalScale);

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


                            //g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.Inner.CenterH - valueL, 1, valueL);
                            //
                            //g.FillRectangle(RightBrush, xPosition, DisplayRectangle.Inner.CenterH, 1, valueR);
                        }
                    }

                    ReadyBitmap.Copy(CacheBitmap);
                    //Task.Delay(1);
                }


                CacheBitmap.Dispose();
                ReadyBitmap.Dispose();
            });

            //Task.Run(() =>
            //{
            //    try
            //    {

            //        Graphics g = Graphics.FromImage(CacheBitmap);


            //        int startSample = (int)Math.Max(DisplayRectangle.InnerLeftNormalized() * FileData.samplesCount, 0);
            //        int endSample = (int)Math.Min(DisplayRectangle.InnerRightNormalized() * FileData.samplesCount, FileData.samplesCount);
            //        int deltaSamples = (int)((float)FileData.sampleRate / PrerunIterations * DisplayRectangle.Relation());
            //        for (int k = 0; k < PrerunIterations; k++)
            //        {
            //            for (int currentSample = startSample + k;
            //                currentSample < endSample;
            //                currentSample += deltaSamples)
            //            {
            //                if (Canceled)
            //                {
            //                    break;
            //                }

            //                int xPosition =
            //                    (int)(DisplayRectangle.Outer.NormalizedWidth(
            //                              currentSample / (float)FileData.samplesCount) - DisplayRectangle.DeltaLeft());

            //                int valueL =
            //                    (int)(FileData.LeftChannel[currentSample] * (DisplayRectangle.Inner.CenterH) * VerticalScale);
            //                int valueR =
            //                    (int)(FileData.RightChannel[currentSample] * (DisplayRectangle.Inner.CenterH) * VerticalScale);

            //                lock (g)
            //                {
            //                    g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.Inner.CenterH - valueL, 1, valueL);

            //                    g.FillRectangle(RightBrush, xPosition, DisplayRectangle.Inner.CenterH, 1, valueR);
            //                }
            //            }
            //        }

            //        //for (int currentSample = startSample; currentSample < endSample; currentSample++)
            //        //{
            //        //    if (Canceled) break;
            //        //    int xPosition =
            //        //        (int)(DisplayRectangle.Outer.NormalizedWidth(
            //        //                  currentSample / (float)FileData.samplesCount) - DisplayRectangle.DeltaLeft());
            //        //    //int inInnerxPosition = (int) (inOuterxPosition - DisplayRectangle.DeltaLeft());
            //        //
            //        //    int valueL =
            //        //        (int)(FileData.LeftChannel[currentSample] * (DisplayRectangle.Inner.CenterH) *
            //        //              VerticalScale);
            //        //    int valueR =
            //        //        (int)(FileData.RightChannel[currentSample] * (DisplayRectangle.Inner.CenterH) *
            //        //              VerticalScale);
            //        //
            //        //    g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.Inner.CenterH - valueL, 1, valueL);
            //        //
            //        //    g.FillRectangle(RightBrush, xPosition, DisplayRectangle.Inner.CenterH, 1, valueR);
            //        //}

            //        g.Dispose();
            //    }
            //    catch 
            //    {
            //    }
            //});
        }

        public override void Dispose()
        {
            CacheBitmap.Dispose();
            ReadyBitmap.Dispose();
        }
    }
}