using System;

namespace WavVisualize
{
    public class FastLog10Provider
    {
        public static float FastLog10(int i)
        {
            if (i == 0)
            {
                return 0;
            }
            return (float)Math.Log10(i);
        }
    }
}
