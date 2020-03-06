using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class ParallelSpectrumDiagramDrawer : SpectrumDiagramDrawer
    {
        public ParallelSpectrumDiagramDrawer(int spectrumSamples, Rectangle displayRectangle, WavFileData fileData) : base(
            spectrumSamples, displayRectangle, fileData)
        {
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(Diagram.Bitmap, DisplayRectangle.Left, DisplayRectangle.Top, DisplayRectangle.Width,
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
                Parallel.For(0, (int) DisplayRectangle.Width, (i, loopState) =>
                {
                    float[] heightPixels = new float[(int) DisplayRectangle.Height + 1];
                    if (Canceled)
                    {
                        loopState.Break();
                        return;
                    }

                    float realPosition = Math.Min(i / DisplayRectangle.Width,
                        (float) (FileData.samplesCount - SpectrumSamples) / FileData.samplesCount);

                    float[] spectrum;
                    lock (FftProvider)
                    {
                        spectrum = FileData.GetSpectrumForPosition(realPosition, FftProvider);
                    }

                    for (int j = 0; j < useSamples; j++)
                    {
                        int yPosition = (int) (DisplayRectangle.Height -
                                               DisplayRectangle.NormalizedHeight((float) j / useSamples));
                        heightPixels[yPosition] = 100f * spectrum[j] * FastLog10Provider.FastLog10(j);
                    }

                    for (int j = 0; j < (int) DisplayRectangle.Height; j++)
                    {
                        Diagram.SetPixel(i, j, IntensityToArgb(heightPixels[j]));
                    }
                });
            });
        }
    }
}