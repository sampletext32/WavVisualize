using System;
using System.Drawing;

namespace WavVisualize
{
    public abstract class WaveformProvider : IDisposable
    {
        protected Color LeftColor;
        protected Color RightColor;

        protected WavFileData FileData;
        
        //сдвигается ли волна во времени
        public bool IsWaveformScannable = false;

        protected float VerticalScale;

        protected NestedRectangle DisplayRectangle;
        
        public abstract void Draw(Graphics g);
        public abstract void Recreate();

        protected bool Canceled;

        public void Cancel()
        {
            Canceled = true;
        }

        public WaveformProvider(NestedRectangle displayRectangle, Color colorL, Color colorR,
            WavFileData fileData, float verticalScale, bool isWaveformScannable)
        {
            DisplayRectangle = displayRectangle;

            LeftColor = colorL;
            RightColor = colorR;

            FileData = fileData;
            VerticalScale = verticalScale;
            IsWaveformScannable = isWaveformScannable;
        }

        public abstract void Dispose();
    }
}