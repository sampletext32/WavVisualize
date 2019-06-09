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
                        int xPosition = (int) (k / (float) FileData.SamplesCount * DisplayRectangle.Width);

                        int valueL =
                            (int) (FileData.LeftChannel[k] * (DisplayRectangle.Height / 2) * VerticalScale);
                        int valueR =
                            (int) (FileData.RightChannel[k] * (DisplayRectangle.Height / 2) * VerticalScale);

                        g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.Height / 2 - valueL, 1, valueL);

                        g.FillRectangle(RightBrush, xPosition, DisplayRectangle.Height / 2, 1, valueR);
                    }
                }
            });
        }
    }
}