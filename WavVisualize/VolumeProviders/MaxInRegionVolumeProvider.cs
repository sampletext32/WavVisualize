using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class MaxInRegionVolumeProvider : VolumeProvider
    {
        protected override void Algorithm(int start)
        {
            for (int i = 0; i < RegionLength; i++)
            {
                if (LeftChannel[start + i] > CacheL)
                {
                    CacheL = LeftChannel[start + i];
                }

                if (RightChannel[start + i] > CacheR)
                {
                    CacheR = RightChannel[start + i];
                }
            }
        }

        public MaxInRegionVolumeProvider(float[] leftChannel, float[] rightChannel, int regionLength) : base(
            leftChannel, rightChannel, regionLength)
        {
        }
    }
}