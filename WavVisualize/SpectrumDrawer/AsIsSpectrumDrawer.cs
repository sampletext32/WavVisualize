using System.Drawing;

namespace WavVisualize
{
    public class AsIsSpectrumDrawer : SpectrumDrawer
    {
        protected override void InnerDraw(Graphics g, int useLength)
        {
            for (int i = 0; i < useLength; i++)
            {
                float x = DisplayRectangle.NormalizedWidth((float) i / useLength);
                float height =
                    DisplayRectangle.NormalizedHeight(SpectrumValues[i] * FastLog10Provider.FastLog10(i) * ConstantHeightMultiplier);
                g.FillRectangle(Brush, DisplayRectangle.Left + x, DisplayRectangle.Bottom - height, 1, height);
            }
        }

        public AsIsSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, Rectangle displayRectangle,
            Color color) : base(spectrumSamples, constantHeightMultiplier, displayRectangle, color)
        {
        }
    }
}