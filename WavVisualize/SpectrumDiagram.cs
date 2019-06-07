using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class SpectrumDiagram
    {
        protected int SpectrumSamples;
        protected float[] SpectrumValues;

        protected float Left;
        protected float Right;
        protected float Top;
        protected float Bottom;

        protected float Width;
        protected float Height;

        protected FFTProvider FftProvider;
        protected WavFileData FileData;

        private Bitmap Diagram;

        protected int TrimmingFrequency;
        protected bool ApplyTimeThinning;

        private bool _cancel;

        public void SetTrimmingFrequency(int frequency)
        {
            TrimmingFrequency = frequency;
        }

        public void SetApplyTimeThinning(bool apply)
        {
            ApplyTimeThinning = apply;
            FftProvider = new CorrectCooleyTukeyInPlaceFFTProvider(SpectrumSamples, ApplyTimeThinning);
        }

        Brush GetBrush(float intensity)
        {
            if (intensity > 1)
            {
                intensity = 1;
            }

            return new SolidBrush(Color.FromArgb((int) (10  + intensity * (255 - 10)), Color.OrangeRed));
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(Diagram, Left, Top, Width, Height);
        }

        protected float Log10Normalizing(int i)
        {
            return (float) Math.Log10(i);
        }

        public void Recreate()
        {
            Task.Run(() =>
            {
                Graphics g = Graphics.FromImage(Diagram);

                int useSamples = (int) (SpectrumSamples / 2 * TrimmingFrequency / 20000f);
                if (ApplyTimeThinning)
                {
                    useSamples /= 2;
                }

                float[] heightPixels = new float[(int) Height + 1];
                int lastX = 0;
                for (int i = 0; i < Width; i++)
                {
                    if (_cancel)
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

        public void Cancel()
        {
            _cancel = true;
        }

        public SpectrumDiagram(int spectrumSamples, float left, float right,
            float top, float bottom, WavFileData fileData)
        {
            SpectrumSamples = spectrumSamples;
            SpectrumValues = new float[SpectrumSamples];
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;

            Width = right - left;
            Height = bottom - top;

            Diagram = new Bitmap((int) Width, (int) Height);

            FftProvider = new CorrectCooleyTukeyInPlaceFFTProvider(SpectrumSamples, ApplyTimeThinning);
            FileData = fileData;
        }
    }
}