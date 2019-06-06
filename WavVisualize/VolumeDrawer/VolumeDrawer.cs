using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public abstract class VolumeDrawer
    {
        protected float NormalizedVolumeL;
        protected float NormalizedVolumeR;

        protected Brush LeftBrush;
        protected Brush RightBrush;

        protected float Left;
        protected float Right;
        protected float Top;
        protected float Bottom;

        protected float Width;
        protected float Height;

        public void LoadVolume(float volumeL, float volumeR, float easing)
        {
            NormalizedVolumeL = EasingProvider.Ease(NormalizedVolumeL, volumeL, easing);
            NormalizedVolumeR = EasingProvider.Ease(NormalizedVolumeR, volumeR, easing);
        }

        public abstract void Draw(Graphics g);

        public VolumeDrawer(float left, float right, float top, float bottom, Color colorL, Color colorR)
        {
            NormalizedVolumeL = 0;
            NormalizedVolumeR = 0;

            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;

            Width = right - left;
            Height = bottom - top;

            LeftBrush = new SolidBrush(colorL);
            RightBrush = new SolidBrush(colorR);
        }

        ~VolumeDrawer()
        {
            LeftBrush.Dispose();
            RightBrush.Dispose();
        }
    }
}
