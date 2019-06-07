using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WavVisualize
{
    public abstract class WaveformProvider
    {
        protected Color LeftColor;
        protected Color RightColor;

        protected WavFileData FileData;

        protected float VerticalScale;

        protected float Left;
        protected float Right;
        protected float Top;
        protected float Bottom;

        protected float Width;
        protected float Height;

        public abstract void Draw(Graphics g);
        public abstract void Recreate();

        protected bool Canceled;

        public void Cancel()
        {
            Canceled = true;
        }

        public WaveformProvider(float left, float right, float top, float bottom, Color colorL, Color colorR,
            WavFileData fileData, float verticalScale)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;

            Width = right - left;
            Height = bottom - top;

            LeftColor = colorL;
            RightColor = colorR;

            FileData = fileData;
            VerticalScale = verticalScale;
        }
    }
}