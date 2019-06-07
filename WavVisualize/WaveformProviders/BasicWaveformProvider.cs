using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class BasicWaveformProvider : WaveformProvider
    {
        protected Bitmap CacheBitmap;
        protected Brush LeftBrush;
        protected Brush RightBrush;


        public BasicWaveformProvider(float left, float right, float top, float bottom, Color colorL, Color colorR,
            WavFileData fileData, float verticalScale) : base(left, right, top, bottom, colorL, colorR, fileData,
            verticalScale)
        {
            CacheBitmap = new Bitmap((int) Width, (int) Height);
            LeftBrush = new SolidBrush(LeftColor);
            RightBrush = new SolidBrush(RightColor);
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(CacheBitmap, 0, 0, Width, Height);
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
                        int xPosition = (int) (k / (float) FileData.SamplesCount * Width);

                        int valueL =
                            (int) (FileData.LeftChannel[k] * (Height / 2) * VerticalScale);
                        int valueR =
                            (int) (FileData.RightChannel[k] * (Height / 2) * VerticalScale);

                        g.FillRectangle(LeftBrush, xPosition, Height / 2 - valueL, 1, valueL);

                        g.FillRectangle(RightBrush, xPosition, Height / 2, 1, valueR);
                    }
                }
            });
        }
    }
}