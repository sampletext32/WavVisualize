using System.Drawing;

namespace WavVisualize
{
    public abstract class WaveformProvider
    {
        protected Color LeftColor;
        protected Color RightColor;

        protected WavFileData FileData;

        protected float VerticalScale;

        protected Rectangle DisplayRectangle;
        
        public abstract void Draw(Graphics g);
        public abstract void Recreate();

        protected bool Canceled;

        public void Cancel()
        {
            Canceled = true;
        }

        public WaveformProvider(Rectangle displayRectangle, Color colorL, Color colorR,
            WavFileData fileData, float verticalScale)
        {
            DisplayRectangle = displayRectangle;

            LeftColor = colorL;
            RightColor = colorR;

            FileData = fileData;
            VerticalScale = verticalScale;
        }
    }
}