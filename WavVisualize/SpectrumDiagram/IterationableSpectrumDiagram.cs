﻿using System;
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

            int frequencyHeight = 1;
            if (useSamples < DisplayRectangle.Height)
            {
                frequencyHeight = (int) Math.Ceiling(DisplayRectangle.Height / useSamples);
            }

            Task.Run(() =>
            {
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
                            int argb = IntensityToArgb(heightPixels[j]);

                            for (int m = 0; m < frequencyHeight; m++)
                            {
                                Diagram.SetPixel(i, j + m, argb);
                            }
                        }
                    }
                });
            });
        }
    }
}