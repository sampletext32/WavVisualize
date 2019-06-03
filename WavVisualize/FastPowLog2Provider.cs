using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class FastPowLog2Provider
    {
        public static int FastLog2(int n)
        {
            switch (n)
            {
                case 1: return 0;
                case 2: return 1;
                case 4: return 2;
                case 8: return 3;
                case 16: return 4;
                case 32: return 5;
                case 64: return 6;
                case 128: return 7;
                case 256: return 8;
                case 512: return 9;
                case 1024: return 10;
                case 2048: return 11;
                case 4096: return 12;
                case 8192: return 13;
                case 16384: return 14;
                case 32768: return 15;
                case 65536: return 16;
                case 131_072: return 17;
            }

            return -1;
        }

        public static int FastPow2(int n)
        {
            switch (n)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 4;
                case 3: return 8;
                case 4: return 16;
                case 5: return 32;
                case 6: return 64;
                case 7: return 128;
                case 8: return 256;
                case 9: return 512;
                case 10: return 1024;
                case 11: return 2048;
                case 12: return 4096;
                case 13: return 8192;
                case 14: return 16384;
                case 15: return 32768;
                case 16: return 65536;
                case 17: return 131_072;
            }

            return -1;
        }
    }
}