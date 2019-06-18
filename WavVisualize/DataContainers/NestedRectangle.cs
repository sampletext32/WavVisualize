using System.Windows.Forms;

namespace WavVisualize
{
    public class NestedRectangle
    {
        public Rectangle Outer;
        public Rectangle Inner;

        public float InnerLeftNormalized()
        {
            return (Inner.Left - Outer.Left) / Outer.Width;
        }

        public float InnerRightNormalized()
        {
            return 1 - (Outer.Right - Inner.Right) / Outer.Width;
        }

        public void SetInnerCenterAt(float normal)
        {
            float newX = Outer.Left + Outer.NormalizedWidth(normal);
            Outer.ShiftX(Inner.CenterW - newX);
        }

        public float DeltaLeft()
        {
            return Inner.Left - Outer.Left;
        }

        public float Relation()
        {
            return Inner.Width / Outer.Width;
        }

        public static NestedRectangle FromPictureBox(PictureBox pb)
        {
            Rectangle region = Rectangle.FromPictureBox(pb);
            Rectangle outer = Rectangle.FromPictureBox(pb);
            return new NestedRectangle(region, outer);
        }

        public NestedRectangle(Rectangle outer, Rectangle inner)
        {
            Outer = outer;
            Inner = inner;
        }
    }
}