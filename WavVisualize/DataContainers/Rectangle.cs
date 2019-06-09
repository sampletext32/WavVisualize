using System.Windows.Forms;

namespace WavVisualize
{
    public class Rectangle
    {
        private float _left;
        private float _right;
        private float _top;
        private float _bottom;

        public float Left
        {
            get => _left;
            set
            {
                _left = value;
                RecalculateWidthAndHeight();
            }
        }

        public float Right
        {
            get => _right;
            set
            {
                _right = value;
                RecalculateWidthAndHeight();
            }
        }

        public float Top
        {
            get => _top;
            set
            {
                _top = value;
                RecalculateWidthAndHeight();
            }
        }

        public float Bottom
        {
            get => _bottom;
            set
            {
                _bottom = value;
                RecalculateWidthAndHeight();
            }
        }

        public float Width { get; private set; }

        public float Height { get; private set; }

        private void RecalculateWidthAndHeight()
        {
            Width = Right - Left;
            Height = Bottom - Top;
        }

        public static Rectangle FromPictureBox(PictureBox pictureBox)
        {
            return new Rectangle(0, pictureBox.Width, 0, pictureBox.Height);
        }

        public Rectangle(float left, float right, float top, float bottom)
        {
            _left = left;
            _right = right;
            _top = top;
            _bottom = bottom;
            RecalculateWidthAndHeight();
        }
    }
}