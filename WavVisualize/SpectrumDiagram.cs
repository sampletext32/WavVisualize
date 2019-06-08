using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public abstract class SpectrumDiagram
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

        protected Bitmap Diagram;

        protected int TrimmingFrequency;
        protected bool ApplyTimeThinning;

        protected bool Canceled;

        public void SetTrimmingFrequency(int frequency)
        {
            TrimmingFrequency = frequency;
        }

        public void SetApplyTimeThinning(bool apply)
        {
            ApplyTimeThinning = apply;
            FftProvider = new CorrectCooleyTukeyInPlaceFFTProvider(SpectrumSamples, ApplyTimeThinning);
        }

        protected Brush GetBrush(float intensity)
        {
            if (intensity > 1)
            {
                intensity = 1;
            }

            if (intensity < 0)
            {
                intensity = 0;
            }

            if (float.IsNaN(intensity))
            {
                intensity = 0;
            }

            return new SolidBrush(Color.FromArgb((int) (10  + intensity * (255 - 10)), Color.OrangeRed));
        }

        public abstract void Draw(Graphics g);

        protected float Log10Normalizing(int i)
        {
            return (float) Math.Log10(i);
        }

        public abstract void Recreate();

        public void Cancel()
        {
            Canceled = true;
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