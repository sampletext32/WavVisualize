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

                        int xPosition = (int)DisplayRectangle.NormalizedWidth(i / (float)FileData.SamplesCount);

                        int valueL =
                            (int) (FileData.LeftChannel[i] * (DisplayRectangle.CenterH) * VerticalScale);
                        int valueR =
                            (int) (FileData.RightChannel[i] * (DisplayRectangle.CenterH) * VerticalScale);

                        lock (g)
                        {
                            g.FillRectangle(LeftBrush, xPosition, DisplayRectangle.CenterH - valueL, 1, valueL);

                            g.FillRectangle(RightBrush, xPosition, DisplayRectangle.CenterH, 1, valueR);
                        }
                    }
                }

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

                g.Dispose();
            });
        }
    }
}