using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class AsIsSpectrumDrawer : SpectrumDrawer
    {
        protected override void InnerDraw(Graphics g, int useLength)
        {
            for (int i = 0; i < useLength; i++)
            {
                float x = (float)i / useLength * Width;
                float value = SpectrumValues[i] * Log10Normalizing(i) * Height * ConstantHeightMultiplier;
                g.FillRectangle(Brush, Left + x, Bottom - value, 1, value);
            }
        }

        public AsIsSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, float left, float right, float top, float bottom, Color color) : base(spectrumSamples, constantHeightMultiplier, left, right, top, bottom, color)
        {
        }
    }
}