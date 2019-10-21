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
            Color colorR, WavFileData fileData, float verticalScale, int prerunIterations) : base(displayRectangle,
            colorL, colorR,
            fileData, verticalScale)
        {
            CacheBitmap = new DirectBitmap((int)displayRectangle.Inner.Width, (int)displayRectangle.Inner.Height);
            ReadyBitmap = new DirectBitmap((int)displayRectangle.Inner.Width, (int)displayRectangle.Inner.Height);
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
                while (!Canceled)
                {
                    //Graphics g = Graphics.FromImage(CacheBitmap.Bitmap);
                    //g.Clear(Color.White);
                    CacheBitmap.Clear();

                    int startSample = (int)Math.Max(DisplayRectangle.InnerLeftNormalized() * FileData.SamplesCount, 0);
                    int endSample = (int)Math.Min(DisplayRectangle.InnerRightNormalized() * FileData.SamplesCount,
                        FileData.SamplesCount);
                    int deltaSamples =
                        (int)((float)FileData.SampleRate / PrerunIterations * DisplayRectangle.Relation());
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
                                (int)(DisplayRectangle.Outer.NormalizedWidth(
                                           currentSample / (float)FileData.SamplesCount) -
                                       DisplayRectangle.DeltaLeft());
                            if (xPosition != lastX)
                            {
                                maxValL = 0;
                                maxValR = 0;
                            }

                            lastX = xPosition;

                            int valueL =
                                (int)(FileData.LeftChannel[currentSample] * (DisplayRectangle.Inner.CenterH) *
                                       VerticalScale);

                            if (valueL > maxValL)
                            {
                                for (float y = DisplayRectangle.Inner.CenterH - valueL;
                                    y < DisplayRectangle.Inner.CenterH - maxValR;
                                    y++)
                                {
                                    CacheBitmap.SetPixel(xPosition, (int)y, leftArgb);
                                }
                                maxValL = valueL;
                            }



                            int valueR =
                                (int)(FileData.RightChannel[currentSample] * (DisplayRectangle.Inner.CenterH) *
                                       VerticalScale);

                            if (valueR > maxValR)
                            {
                                for (float y = DisplayRectangle.Inner.CenterH + maxValR;
                                    y < DisplayRectangle.Inner.CenterH + valueR;
                                    y++)
                                {
                                    CacheBitmap.SetPixel(xPosition, (int)y, rightArgb);
                                }
                                maxValR = valueR;
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


            //        int startSample = (int)Math.Max(DisplayRectangle.InnerLeftNormalized() * FileData.SamplesCount, 0);
            //        int endSample = (int)Math.Min(DisplayRectangle.InnerRightNormalized() * FileData.SamplesCount, FileData.SamplesCount);
            //        int deltaSamples = (int)((float)FileData.SampleRate / PrerunIterations * DisplayRectangle.Relation());
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
            //                              currentSample / (float)FileData.SamplesCount) - DisplayRectangle.DeltaLeft());

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
            //        //                  currentSample / (float)FileData.SamplesCount) - DisplayRectangle.DeltaLeft());
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