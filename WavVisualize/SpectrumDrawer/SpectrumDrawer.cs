using System;
using System.Drawing;

namespace WavVisualize
{
    public abstract class SpectrumDrawer
    {
        protected int SpectrumSamples;
        protected float[] SpectrumValues;

        protected Rectangle DisplayRectangle;
        protected Brush Brush;

        protected float ConstantHeightMultiplier;

        protected int TrimmingFrequency;

        protected bool ApplyTimeThinning;

        public void SetTrimmingFrequency(int frequency)
        {
            TrimmingFrequency = frequency;
        }

        public void SetApplyTimeThinning(bool apply)
        {
            ApplyTimeThinning = apply;
        }

        public void LoadSpectrum(float[] spectrum, float easing)
        {
            EasingProvider.Ease(SpectrumValues, spectrum, easing);
        }

        public void Draw(Graphics g)
        {
            int useSamples;
            useSamples = SpectrumSamples >> 1; // as FFT produces mirrored values, we cut exactly half
            if (ApplyTimeThinning)
            {
                useSamples >>= 2; // as timeThinning adds even zeros, there are only half of actual signals
            }

            useSamples = (int) (useSamples * TrimmingFrequency / 20000f);

            InnerDraw(g, useSamples);
        }

        protected abstract void InnerDraw(Graphics g, int useLength);

        protected SpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, Rectangle displayRectangle,
            Color color)
        {
            SpectrumSamples = spectrumSamples;
            ConstantHeightMultiplier = constantHeightMultiplier;
            SpectrumValues = new float[SpectrumSamples];
            DisplayRectangle = displayRectangle;
            Brush = new SolidBrush(color);
        }

        ~SpectrumDrawer()
        {
            Brush.Dispose();
        }
    }
}