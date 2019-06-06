using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class TimeProvider
    {
        public static Tuple<int, int, int> SecondsAsTime(float seconds)
        {
            int s = (int)seconds % 60;
            seconds /= 60;
            int m = (int)seconds % 60;
            seconds /= 60;
            int h = (int) seconds % 3600;
            return new Tuple<int, int, int>(h, m, s);
        }
    }
}