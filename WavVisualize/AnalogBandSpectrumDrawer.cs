using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class AnalogBandSpectrumDrawer : BandBasedSpectrumDrawer
    {
        

        protected override void DrawBand(Graphics g, int band, float value)
        {
            g.FillRectangle(Brush, Left + band * (BandWidth + DistanceBetweenBands),
                Bottom - value,
                BandWidth, value);
        }

        public AnalogBandSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, float left, float right, float top, float bottom, Color color, int bandsCount, float distanceBetweenBands) : base(spectrumSamples, constantHeightMultiplier, left, right, top, bottom, color, bandsCount, distanceBetweenBands)
        {
        }
    }
}