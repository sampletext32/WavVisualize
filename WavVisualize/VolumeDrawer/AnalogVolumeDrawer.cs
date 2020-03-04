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
            var heightL = DisplayRectangle.NormalizedHeight(NormalizedVolumeL);
            var heightR = DisplayRectangle.NormalizedHeight(NormalizedVolumeR);

            g.FillRectangle(LeftBrush, DisplayRectangle.Left, DisplayRectangle.Bottom - heightL,
                DisplayRectangle.CenterW, heightL);

            g.FillRectangle(RightBrush, DisplayRectangle.CenterW, DisplayRectangle.Bottom - heightR,
                DisplayRectangle.CenterW, heightR);
        }
    }
}