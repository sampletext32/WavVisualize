using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class BasicWithIterationablePrerunWaveformProvider : WaveformProvider
    {
        protected Bitmap CacheBitmap;
        protected Brush LeftBrush;
        protected Brush RightBrush;
        protected int PrerunIterations;

        public BasicWithIterationablePrerunWaveformProvider(Rectangle displayRectangle,
            Color colorL,
            Color colorR, WavFileData fileData, float verticalScale, int prerunIterations) : base(displayRectangle,
            colorL, colorR,
            fileData, verticalScale)
        {
            CacheBitmap = new Bitmap((int) displayRectangle.Width, (int) displayRectangle.Height);
            LeftBrush = new SolidBrush(LeftColor);
            RightBrush = new SolidBrush(RightColor);

            PrerunIterations = prerunIterations;
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(CacheBitmap, 0, 0, DisplayRectangle.Width, DisplayRectangle.Height);
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

                        int xPosition = (int) (i / (float) FileData.SamplesCount * DisplayRectangle.Width);

                        int valueL =
                            (int) (FileData.LeftChannel[i] * (DisplayRectangle.Height / 2) * VerticalScale);
                        int valueR =
                            (int) (FileData.RightChannel[i] * (DisplayRectangle.Height / 2) * VerticalScale);

                        lock (g)
                        {
                            g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.Height / 2 - valueL, 1, valueL);

                            g.FillRectangle(RightBrush, xPosition, DisplayRectangle.Height / 2, 1, valueR);
                        }
                    }
                }

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

                g.Dispose();
            });
        }
    }
}