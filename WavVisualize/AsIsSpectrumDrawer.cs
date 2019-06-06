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
        public AsIsSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, float left, float right, float top, float bottom, Color color) :
            base(spectrumSamples, constantHeightMultiplier, left, right, top, bottom, color)
        {
        }

        public override void Draw(Graphics g)
        {
            for (int i = 0; i < SpectrumSamples; i++)
            {
                float x = (float) i / SpectrumSamples * Width;
                float value = SpectrumValues[i] * Log10Normalizing(i) * Height * ConstantHeightMultiplier;
                g.FillRectangle(Brush, Left + x, Bottom - value, 1, value);
            }
        }
    }
}