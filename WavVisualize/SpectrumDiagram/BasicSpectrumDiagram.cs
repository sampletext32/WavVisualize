using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class BasicSpectrumDiagram : SpectrumDiagram
    {
        public BasicSpectrumDiagram(int spectrumSamples, Rectangle displayRectangle,
            WavFileData fileData) : base(spectrumSamples, displayRectangle, fileData)
        {
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(Diagram, DisplayRectangle.Left, DisplayRectangle.Top, DisplayRectangle.Width,
                DisplayRectangle.Height);
        }

        public override void Recreate()
        {
            int useSamples = (int) (SpectrumSamples / 2 * TrimmingFrequency / 20000f);
            if (ApplyTimeThinning)
            {
                useSamples /= 2;
            }

            Task.Run(() =>
            {
                Graphics g = Graphics.FromImage(Diagram);

                float[] heightPixels = new float[(int) DisplayRectangle.Height + 1];

                int lastX = 0;
                for (int i = 0; i < DisplayRectangle.Width; i++)
                {
                    if (Canceled)
                    {
                        break;
                    }

                    float realPosition = Math.Min((float) i / DisplayRectangle.Width,
                        (float) (FileData.SamplesCount - SpectrumSamples) / FileData.SamplesCount);

                    float[] spectrum = FileData.GetSpectrumForPosition(realPosition, FftProvider);

                    for (int j = 0; j < (int) DisplayRectangle.Height; j++)
                    {
                        Brush b = GetBrush(heightPixels[j]);
                        g.FillRectangle(b, i, j, 1, 1);
                        b.Dispose();
                    }

                    for (int j = 0; j < useSamples; j++)
                    {
                        int yPosition = (int) (DisplayRectangle.Height -
                                               (int) ((float) j / useSamples * DisplayRectangle.Height));
                        heightPixels[yPosition] = 100f * spectrum[j] * Log10Normalizing(j);
                    }
                }

                g.Dispose();
            });
        }
    }
}