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
                float verticalHalf = DisplayRectangle.Inner.CenterH;
                float verticalQuarter = verticalHalf / 2;
                float verticalThreeQuarters = verticalHalf * 3f / 2f;

                //using (Graphics g = Graphics.FromImage(CacheBitmap.Bitmap))
                {
                    int startSample = (int) Math.Max(DisplayRectangle.InnerLeftNormalized() * FileData.samplesCount, 0);
                    int endSample = (int) Math.Min(DisplayRectangle.InnerRightNormalized() * FileData.samplesCount,
                        FileData.samplesCount);
                    for (int currentSample = startSample; currentSample < endSample; currentSample++)
                    {
                        if (Canceled) break;
                        int xPosition =
                            (int) (DisplayRectangle.Outer.NormalizedWidth(
                                       currentSample / (float) FileData.samplesCount) - DisplayRectangle.DeltaLeft());

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
                                CacheBitmap.SetPixel(xPosition, (int)y, RightColor.ToArgb());
                            }
                        }
                        else
                        {
                            for (float y = verticalThreeQuarters - valueR; y < verticalThreeQuarters; y++)
                            {
                                CacheBitmap.SetPixel(xPosition, (int)y, RightColor.ToArgb());
                            }
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