using System.Windows.Forms;

namespace WavVisualize
{
    public class Rectangle
    {
        private float _left;
        private float _right;
        private float _top;
        private float _bottom;

        public float Width { get; private set; }

        public float Height { get; private set; }

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

        public void ScaleX(float value)
        {
            float width = Width;
            SetWidth(width * value);
        }

        public void ScaleY(float value)
        {
            float height = Height;
            SetHeight(height * value);
        }

        public void SetWidth(float width)
        {
            float centerX = CenterW;
            Left = centerX - width / 2;
            Right = centerX + width / 2;
        }

        public void SetHeight(float height)
        {
            float centerY = CenterH;
            Top = centerY - height / 2;
            Bottom = centerY + height / 2;
        }

        public void ShiftX(float delta)
        {
            Left += delta;
            Right += delta;
        }

        public void ShiftY(float delta)
        {
            Top += delta;
            Bottom += delta;
        }

        public float NormalizedWidth(float v)
        {
            return Width * v;
        }

        public float NormalizedHeight(float v)
        {
            return Height * v;
        }

        public float CenterW
        {
            get { return NormalizedWidth(0.5f); }
        }

        public float CenterH
        {
            get { return NormalizedHeight(0.5f); }
        }

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