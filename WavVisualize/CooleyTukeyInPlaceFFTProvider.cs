using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class CooleyTukeyInPlaceFFTProvider : FFTProvider
    {
        /*
            Performs a Bit Reversal Algorithm on a positive integer 
            for given number of bits
            e.g. 011 with 3 bits is reversed to 110 
        */
        private static int BitReverse(int n, int bits)
        {
            int reversedN = n;
            int count = bits - 1;

            n >>= 1;
            while (n > 0)
            {
                reversedN = (reversedN << 1) | (n & 1);
                count--;
                n >>= 1;
            }

            return ((reversedN << count) & ((1 << bits) - 1));
        }

        protected override void Algorithm()
        {
            int bits = PowLog2Provider.FastLog2(Samples);
            for (int j = 1; j < Samples / 2; j++)
            {
                int swapPos = BitReverse(j, bits);
                var temp = ValuesBuffer[j];
                ValuesBuffer[j] = ValuesBuffer[swapPos];
                ValuesBuffer[swapPos] = temp;
            }

            for (int N = 2; N <= Samples; N <<= 1)
            {
                for (int i = 0; i < Samples; i += N)
                {
                    for (int k = 0; k < N / 2; k++)
                    {
                        int evenIndex = i + k;
                        int oddIndex = i + k + (N / 2);
                        var even = ValuesBuffer[evenIndex];
                        var odd = ValuesBuffer[oddIndex];

                        double term = -2 * Math.PI * k / (double) N;
                        Complex exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;

                        ValuesBuffer[evenIndex] = even + exp;
                        ValuesBuffer[oddIndex] = even - exp;
                    }
                }
            }
        }

        public CooleyTukeyInPlaceFFTProvider(int samples) : base(samples)
        {
        }
    }
}