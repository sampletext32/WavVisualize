using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class AnalogVolumeDrawer : VolumeDrawer
    {
        public AnalogVolumeDrawer(float left, float right, float top, float bottom, Color colorL, Color colorR) : base(
            left, right, top, bottom, colorL, colorR)
        {
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(LeftBrush, Left, Bottom - NormalizedVolumeL * Height, Width / 2, NormalizedVolumeL * Height);
            g.FillRectangle(RightBrush, Left + Width / 2, Bottom - NormalizedVolumeR * Height, Width / 2, NormalizedVolumeR * Height);
        }
    }
}