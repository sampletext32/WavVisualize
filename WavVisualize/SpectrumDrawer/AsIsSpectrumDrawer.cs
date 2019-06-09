using System.Drawing;

namespace WavVisualize
{
    public class AsIsSpectrumDrawer : SpectrumDrawer
    {
        protected override void InnerDraw(Graphics g, int useLength)
        {
            for (int i = 0; i < useLength; i++)
            {
                float x = (float) i / useLength * DisplayRectangle.Width;
                float value = SpectrumValues[i] * Log10Normalizing(i) * DisplayRectangle.Height *
                              ConstantHeightMultiplier;
                g.FillRectangle(Brush, DisplayRectangle.Left + x, DisplayRectangle.Bottom - value, 1, value);
            }
        }

        public AsIsSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, Rectangle displayRectangle,
            Color color) : base(spectrumSamples, constantHeightMultiplier, displayRectangle, color)
        {
        }
    }
}