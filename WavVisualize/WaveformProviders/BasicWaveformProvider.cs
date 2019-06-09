using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class BasicWaveformProvider : WaveformProvider
    {
        protected Bitmap CacheBitmap;
        protected Brush LeftBrush;
        protected Brush RightBrush;


        public BasicWaveformProvider(Rectangle displayRectangle, Color colorL, Color colorR,
            WavFileData fileData, float verticalScale) : base(displayRectangle, colorL, colorR, fileData,
            verticalScale)
        {
            CacheBitmap = new Bitmap((int) displayRectangle.Width, (int) displayRectangle.Height);
            LeftBrush = new SolidBrush(LeftColor);
            RightBrush = new SolidBrush(RightColor);
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(CacheBitmap, 0, 0, DisplayRectangle.Width, DisplayRectangle.Height);
        }

        public override void Recreate()
        {
            Task.Run(() =>
            {
                using (Graphics g = Graphics.FromImage(CacheBitmap))
                {
                    for (int k = 0; k < FileData.SamplesCount; k++)
                    {
                        if (Canceled) break;
                        int xPosition = (int)DisplayRectangle.NormalizedWidth(k / (float)FileData.SamplesCount);

                        int valueL =
                            (int) (FileData.LeftChannel[k] * (DisplayRectangle.CenterH) * VerticalScale);
                        int valueR =
                            (int) (FileData.RightChannel[k] * (DisplayRectangle.CenterH) * VerticalScale);

                        g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.CenterH - valueL, 1, valueL);

                        g.FillRectangle(RightBrush, xPosition, DisplayRectangle.CenterH, 1, valueR);
                    }
                }
            });
        }
    }
}