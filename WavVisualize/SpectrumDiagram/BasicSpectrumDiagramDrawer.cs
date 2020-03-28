using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class BasicSpectrumDiagramDrawer : SpectrumDiagramDrawer
    {
        public BasicSpectrumDiagramDrawer(int spectrumSamples, Rectangle displayRectangle,
            WavFileData fileData) : base(spectrumSamples, displayRectangle, fileData)
        {
        }

        public override void Draw(Graphics g)
        {
            g.DrawImageUnscaled(Diagram, 0, 0);
        }

        public override void Recreate()
        {
            int useSamples = SpectrumSamples >> 1;

            if (ApplyTimeThinning)
            {
                useSamples >>= 1;
            }

            useSamples = (int) (useSamples * TrimmingFrequency / 20000f);

            Task.Run(() =>
            {
                float[] heightPixels = new float[(int) DisplayRectangle.Height + 1];

                int lastX = 0;
                for (int i = 0; i < DisplayRectangle.Width; i++)
                {
                    if (Canceled)
                    {
                        break;
                    }

                    float realPosition = Math.Min(i / DisplayRectangle.Width,
                        (float) (FileData.samplesCount - SpectrumSamples) / FileData.samplesCount);

                    float[] spectrum = FileData.GetSpectrumForPosition(realPosition, FftProvider);
                    for (int j = 0; j < useSamples; j++)
                    {
                        int yPosition = (int) (DisplayRectangle.Height -
                                               DisplayRectangle.NormalizedHeight((float) j / useSamples));
                        heightPixels[yPosition] = spectrum[j];
                    }

                    for (int j = 0; j < (int) DisplayRectangle.Height; j++)
                    {
                        Diagram.SetPixel(i, j, IntensityToArgb(heightPixels[j]));
                    }
                }
            });
        }
    }
}