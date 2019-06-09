namespace WavVisualize
{
    public abstract class VolumeProvider
    {
        protected float[] LeftChannel;
        protected float[] RightChannel;
        protected int RegionLength;

        protected float CurrentVolumeL;
        protected float CurrentVolumeR;

        protected float CacheL;
        protected float CacheR;

        public float GetL()
        {
            return CurrentVolumeL;
        }

        public float GetR()
        {
            return CurrentVolumeR;
        }

        protected void ResetCache()
        {
            CacheL = 0f;
            CacheR = 0f;
        }

        protected void LoadCache()
        {
            CurrentVolumeL = CacheL;
            CurrentVolumeR = CacheR;
        }

        protected abstract void Algorithm(int start);

        public void Calculate(int start)
        {
            ResetCache();

            Algorithm(start);

            LoadCache();
        }

        protected VolumeProvider(float[] leftChannel, float[] rightChannel, int regionLength)
        {
            LeftChannel = leftChannel;
            RightChannel = rightChannel;
            RegionLength = regionLength;
        }
    }
}