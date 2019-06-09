using System.Drawing;

namespace WavVisualize
{
    public class AnalogVolumeDrawer : VolumeDrawer
    {
        public AnalogVolumeDrawer(Rectangle displayRectangle, Color colorL, Color colorR) : base(displayRectangle, colorL, colorR)
        {
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(LeftBrush, DisplayRectangle.Left, DisplayRectangle.Bottom - NormalizedVolumeL * DisplayRectangle.Height, DisplayRectangle.Width / 2, NormalizedVolumeL * DisplayRectangle.Height);
            g.FillRectangle(RightBrush, DisplayRectangle.Left + DisplayRectangle.Width / 2, DisplayRectangle.Bottom - NormalizedVolumeR * DisplayRectangle.Height, DisplayRectangle.Width / 2, NormalizedVolumeR * DisplayRectangle.Height);
        }
    }
}