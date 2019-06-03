using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class AverageInRegionVolumeProvider : VolumeProvider
    {
        protected override void Algorithm(int start)
        {
            for (int i = 0; i < RegionLength; i++)
            {
                CacheL += LeftChannel[start + i];
                CacheR += RightChannel[start + i];
            }

            //нормализуем обе громкости
            CacheL /= RegionLength;
            CacheR /= RegionLength;
        }

        public AverageInRegionVolumeProvider(float[] leftChannel, float[] rightChannel, int regionLength) : base(
            leftChannel, rightChannel, regionLength)
        {
        }
    }
}