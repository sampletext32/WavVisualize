using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    abstract class SpectrumDrawer
    {
        protected int SpectrumSamples;
        protected float[] SpectrumValues;

        protected float Left;
        protected float Right;
        protected float Top;
        protected float Bottom;

        protected float Width;
        protected float Height;

        protected Brush Brush;

        protected float ConstantHeightMultiplier;

        protected float Log10Normalizing(int i)
        {
            return (float) Math.Log10(i);
        }

        public void LoadSpectrum(float[] spectrum, float easing)
        {
            EasingProvider.Ease(SpectrumValues, spectrum, easing);
        }

        public abstract void Draw(Graphics g);

        protected SpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, float left, float right, float top, float bottom, Color color)
        {
            SpectrumSamples = spectrumSamples;
            ConstantHeightMultiplier = constantHeightMultiplier;
            SpectrumValues = new float[SpectrumSamples];
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;

            Width = right - left;
            Height = bottom - top;

            Brush = new SolidBrush(color);
        }

        ~SpectrumDrawer()
        {
            Brush.Dispose();
        }
    }
}