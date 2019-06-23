using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class BasicWithIterationablePrerunWaveformProvider : WaveformProvider
    {
        protected DirectBitmap CacheBitmap;
        protected Brush LeftBrush;
        protected Brush RightBrush;
        protected int PrerunIterations;

        public BasicWithIterationablePrerunWaveformProvider(NestedRectangle displayRectangle,
            Color colorL,
            Color colorR, WavFileData fileData, float verticalScale, int prerunIterations) : base(displayRectangle,
            colorL, colorR,
            fileData, verticalScale)
        {
            CacheBitmap = new DirectBitmap((int) displayRectangle.Inner.Width, (int) displayRectangle.Inner.Height);
            LeftBrush = new SolidBrush(LeftColor);
            RightBrush = new SolidBrush(RightColor);

            PrerunIterations = prerunIterations;
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(CacheBitmap.Bitmap, 0, 0, DisplayRectangle.Inner.Width, DisplayRectangle.Inner.Height);
        }

        public override void Recreate()
        {
            Graphics g = Graphics.FromImage(CacheBitmap.Bitmap);


            int startSample = (int) Math.Max(DisplayRectangle.InnerLeftNormalized() * FileData.SamplesCount, 0);
            int endSample = (int) Math.Min(DisplayRectangle.InnerRightNormalized() * FileData.SamplesCount,
                FileData.SamplesCount);
            int deltaSamples = (int) ((float) FileData.SampleRate / PrerunIterations * DisplayRectangle.Relation());
            startSample += deltaSamples - startSample % deltaSamples;
            for (int k = 0; k < PrerunIterations; k++)
            {
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
                                   currentSample / (float) FileData.SamplesCount) - DisplayRectangle.DeltaLeft());

                    int valueL =
                        (int) (FileData.LeftChannel[currentSample] * (DisplayRectangle.Inner.CenterH) * VerticalScale);
                    int valueR =
                        (int) (FileData.RightChannel[currentSample] * (DisplayRectangle.Inner.CenterH) * VerticalScale);

                    g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.Inner.CenterH - valueL, 1, valueL);

                    g.FillRectangle(RightBrush, xPosition, DisplayRectangle.Inner.CenterH, 1, valueR);
                }
            }

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

        }
    }
}