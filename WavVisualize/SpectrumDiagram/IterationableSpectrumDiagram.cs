using System;
using System.Drawing;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class IterationableSpectrumDiagram : SpectrumDiagram
    {
        protected int Iterations;

        public IterationableSpectrumDiagram(int spectrumSamples, Rectangle displayRectangle,
            WavFileData fileData, int iterations) : base(spectrumSamples, displayRectangle, fileData)
        {
            Iterations = iterations;
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

            int frequencyHeight = 1;
            if (useSamples < DisplayRectangle.Height)
            {
                frequencyHeight = (int) Math.Ceiling(DisplayRectangle.Height / useSamples);
            }

            Task.Run(() =>
            {
                Graphics g = Graphics.FromImage(Diagram);


                Parallel.For(0, Iterations, (k, loopState) =>
                {
                    float[] heightPixels = new float[(int) DisplayRectangle.Height + 1];
                    int lastX = 0;
                    for (int i = k; i < DisplayRectangle.Width; i += Iterations)
                    {
                        if (Canceled)
                        {
                            loopState.Break();
                            return;
                        }

                        float realPosition = Math.Min((float) i / DisplayRectangle.Width,
                            (float) (FileData.SamplesCount - SpectrumSamples) / FileData.SamplesCount);


                        float[] spectrum;
                        lock (FftProvider)
                        {
                            spectrum = FileData.GetSpectrumForPosition(realPosition, FftProvider);

                            for (int j = 0; j < useSamples; j++)
                            {
                                int yPosition = (int) (DisplayRectangle.Height -
                                                       (int) ((float) j / useSamples * DisplayRectangle.Height));
                                heightPixels[yPosition] = 100f * spectrum[j] * Log10Normalizing(j);
                            }

                            for (int j = 0; j < (int) DisplayRectangle.Height; j++)
                            {
                                Brush b = GetBrush(heightPixels[j]);
                                lock (g)
                                {
                                    g.FillRectangle(b, i, j, 1, frequencyHeight);
                                }

                                b.Dispose();
                            }
                        }
                    }
                });
                g.Dispose();
            });
        }
    }
}