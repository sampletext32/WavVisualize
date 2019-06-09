using System.Drawing;

namespace WavVisualize
{
    public abstract class VolumeDrawer
    {
        protected float NormalizedVolumeL;
        protected float NormalizedVolumeR;

        protected Brush LeftBrush;
        protected Brush RightBrush;

        protected Rectangle DisplayRectangle;

        public void LoadVolume(float volumeL, float volumeR, float easing)
        {
            NormalizedVolumeL = EasingProvider.Ease(NormalizedVolumeL, volumeL, easing);
            NormalizedVolumeR = EasingProvider.Ease(NormalizedVolumeR, volumeR, easing);
        }

        public abstract void Draw(Graphics g);

        public VolumeDrawer(Rectangle displayRectangle, Color colorL, Color colorR)
        {
            NormalizedVolumeL = 0;
            NormalizedVolumeR = 0;
            DisplayRectangle = displayRectangle;

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