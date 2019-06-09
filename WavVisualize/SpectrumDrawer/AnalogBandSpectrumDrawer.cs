using System.Drawing;

namespace WavVisualize
{
    public class AnalogBandSpectrumDrawer : BandBasedSpectrumDrawer
    {
        protected override void DrawBand(Graphics g, int band, float value)
        {
            g.FillRectangle(Brush, DisplayRectangle.Left + band * (BandWidth + DistanceBetweenBands),
                DisplayRectangle.Bottom - value,
                BandWidth, value);
        }

        public AnalogBandSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, Rectangle displayRectangle,
            Color color, int bandsCount, float distanceBetweenBands) : base(spectrumSamples, constantHeightMultiplier,
            displayRectangle, color, bandsCount, distanceBetweenBands)
        {
        }
    }
}