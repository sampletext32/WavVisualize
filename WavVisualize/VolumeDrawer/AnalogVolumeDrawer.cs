using System.Drawing;

namespace WavVisualize
{
    public class AnalogVolumeDrawer : VolumeDrawer
    {
        public AnalogVolumeDrawer(Rectangle displayRectangle, Color colorL, Color colorR) : base(displayRectangle,
            colorL, colorR)
        {
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(LeftBrush, DisplayRectangle.Left,
                DisplayRectangle.Bottom - DisplayRectangle.NormalizedHeight(NormalizedVolumeL),
                DisplayRectangle.CenterW, DisplayRectangle.NormalizedHeight(NormalizedVolumeL));
            g.FillRectangle(RightBrush, DisplayRectangle.CenterW,
                DisplayRectangle.Bottom - DisplayRectangle.NormalizedHeight(NormalizedVolumeR),
                DisplayRectangle.CenterW, DisplayRectangle.NormalizedHeight(NormalizedVolumeR));
        }
    }
}