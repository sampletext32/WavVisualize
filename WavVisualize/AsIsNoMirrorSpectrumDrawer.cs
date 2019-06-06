using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class AsIsNoMirrorSpectrumDrawer : AsIsSpectrumDrawer
    {
        public AsIsNoMirrorSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, float left, float right,
            float top, float bottom, Color color) : base(spectrumSamples / 2, constantHeightMultiplier, left, right,
            top, bottom, color)
        {
        }
    }
}