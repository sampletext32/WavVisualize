using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class TopLineSpectrumDrawer : SpectrumDrawer
    {
        private Color color_;
        private Pen pen_;
        public TopLineSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, Rectangle displayRectangle,
               Color color) : base(spectrumSamples, constantHeightMultiplier, displayRectangle, color)
        {
            color_ = color;
            pen_ = new Pen(color, 3);
        }

        protected override void InnerDraw(Graphics g, int useLength)
        {
            float lastX = 0f;
            float lastY = DisplayRectangle.Bottom;
            for (int i = 0; i < useLength; i++)
            {
                float x = DisplayRectangle.NormalizedWidth((float)i / useLength);
                float height =
                    DisplayRectangle.NormalizedHeight(SpectrumValues[i] * FastLog10Provider.FastLog10(i) * ConstantHeightMultiplier);
                g.DrawLine(pen_, lastX, lastY, x, DisplayRectangle.Bottom - height);

                lastX = x;
                lastY = DisplayRectangle.Bottom - height;
            }
        }
    }
}
