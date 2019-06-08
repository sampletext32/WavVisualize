using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class IterationableSpectrumDiagram : SpectrumDiagram
    {
        protected int Iterations;

        public IterationableSpectrumDiagram(int spectrumSamples, float left, float right, float top, float bottom,
            WavFileData fileData, int iterations) : base(spectrumSamples, left, right, top, bottom, fileData)
        {
            Iterations = iterations;
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(Diagram, Left, Top, Width, Height);
        }

        public override void Recreate()
        {
            int useSamples = (int) (SpectrumSamples / 2 * TrimmingFrequency / 20000f);
            if (ApplyTimeThinning)
            {
                useSamples /= 2;
            }

            int frequencyHeight = 1;
            if (useSamples < Height)
            {
                frequencyHeight = (int)Math.Ceiling(Height / useSamples);
            }

            Task.Run(() =>
            {
                Graphics g = Graphics.FromImage(Diagram);


                Parallel.For(0, Iterations, (k, loopState) =>
                {
                    float[] heightPixels = new float[(int) Height + 1];
                    int lastX = 0;
                    for (int i = k; i < Width; i += Iterations)
                    {
                        if (Canceled)
                        {
                            loopState.Break();
                            return;
                        }

                        float realPosition = Math.Min((float) i / Width,
                            (float) (FileData.SamplesCount - SpectrumSamples) / FileData.SamplesCount);


                        float[] spectrum;
                        lock (FftProvider)
                        {
                            spectrum = FileData.GetSpectrumForPosition(realPosition, FftProvider);

                            for (int j = 0; j < useSamples; j++)
                            {
                                int yPosition = (int) (Height - (int) ((float) j / useSamples * Height));
                                heightPixels[yPosition] = 100f * spectrum[j] * Log10Normalizing(j);
                            }

                            for (int j = 0; j < (int) Height; j++)
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