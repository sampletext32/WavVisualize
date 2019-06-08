using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class BasicSpectrumDiagram : SpectrumDiagram
    {
        public BasicSpectrumDiagram(int spectrumSamples, float left, float right, float top, float bottom, WavFileData fileData) : base(spectrumSamples, left, right, top, bottom, fileData)
        {
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(Diagram, Left, Top, Width, Height);
        }

        public override void Recreate()
        {
            int useSamples = (int)(SpectrumSamples / 2 * TrimmingFrequency / 20000f);
            if (ApplyTimeThinning)
            {
                useSamples /= 2;
            }

            Task.Run(() =>
            {
                Graphics g = Graphics.FromImage(Diagram);

                float[] heightPixels = new float[(int)Height + 1];

                int lastX = 0;
                for (int i = 0; i < Width; i++)
                {
                    if (Canceled)
                    {
                        break;
                    }
                
                    float realPosition = Math.Min((float) i / Width,
                        (float) (FileData.SamplesCount - SpectrumSamples) / FileData.SamplesCount);
                
                    float[] spectrum = FileData.GetSpectrumForPosition(realPosition, FftProvider);
                
                    for (int j = 0; j < (int) Height; j++)
                    {
                        Brush b = GetBrush(heightPixels[j]);
                        g.FillRectangle(b, i, j, 1, 1);
                        b.Dispose();
                    }
                
                    for (int j = 0; j < useSamples; j++)
                    {
                        int yPosition = (int) (Height - (int) ((float) j / useSamples * Height));
                        heightPixels[yPosition] = 100f * spectrum[j] * Log10Normalizing(j);
                    }
                }

                g.Dispose();
            });
        }
    }
}
