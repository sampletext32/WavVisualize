using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class BasicWaveformProvider : WaveformProvider
    {
        protected DirectBitmap CacheBitmap;
        protected Brush LeftBrush;
        protected Brush RightBrush;


        public BasicWaveformProvider(NestedRectangle displayRectangle, Color colorL, Color colorR,
            WavFileData fileData, float verticalScale) : base(displayRectangle, colorL, colorR, fileData,
            verticalScale)
        {
            CacheBitmap = new DirectBitmap((int) displayRectangle.Inner.Width, (int) displayRectangle.Inner.Height);
            LeftBrush = new SolidBrush(LeftColor);
            RightBrush = new SolidBrush(RightColor);
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(CacheBitmap.Bitmap, 0, 0, DisplayRectangle.Inner.Width, DisplayRectangle.Inner.Height);
        }

        public override void Recreate()
        {
            CacheBitmap.Clear();
            Task.Run(() =>
            {
                //using (Graphics g = Graphics.FromImage(CacheBitmap.Bitmap))
                {
                    int startSample = (int) Math.Max(DisplayRectangle.InnerLeftNormalized() * FileData.SamplesCount, 0);
                    int endSample = (int) Math.Min(DisplayRectangle.InnerRightNormalized() * FileData.SamplesCount, FileData.SamplesCount);
                    for (int currentSample = startSample; currentSample < endSample; currentSample++)
                    {
                        if (Canceled) break;
                        int xPosition =
                            (int) (DisplayRectangle.Outer.NormalizedWidth(
                                       currentSample / (float) FileData.SamplesCount) - DisplayRectangle.DeltaLeft());
                        //int inInnerxPosition = (int) (inOuterxPosition - DisplayRectangle.DeltaLeft());

                        int valueL =
                            (int) (FileData.LeftChannel[currentSample] * (DisplayRectangle.Inner.CenterH) *
                                   VerticalScale);
                        int valueR =
                            (int) (FileData.RightChannel[currentSample] * (DisplayRectangle.Inner.CenterH) *
                                   VerticalScale);

                        for (float y = DisplayRectangle.Inner.CenterH - valueL; y < DisplayRectangle.Inner.CenterH; y++)
                        {
                            CacheBitmap.SetPixel(xPosition, (int)y, LeftColor.ToArgb());
                        }

                        for (float y = DisplayRectangle.Inner.CenterH; y < DisplayRectangle.Inner.CenterH + valueR; y++)
                        {
                            CacheBitmap.SetPixel(xPosition, (int)y, RightColor.ToArgb());
                        }

                        //g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.Inner.CenterH - valueL, 1, valueL);
                        //
                        //g.FillRectangle(RightBrush, xPosition, DisplayRectangle.Inner.CenterH, 1, valueR);
                    }
                }
            });
        }

        public override void Dispose()
        {
            CacheBitmap.Dispose();
        }
    }
}