using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class MyNewFFT
    {
        /*
            Performs a Bit Reversal Algorithm on a positive integer 
            for given number of bits
            e.g. 011 with 3 bits is reversed to 110 
        */
        public static int BitReverse(int n, int bits)
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

        /* Uses Cooley-Tukey iterative in-place algorithm with radix-2 DIT case
         * assumes no of points provided are a power of 2 */
        public static void FFT(Complex[] buffer)
        {
            int bits = (int) Math.Log(buffer.Length, 2);
            for (int j = 1; j < buffer.Length / 2; j++)
            {
                int swapPos = BitReverse(j, bits);
                var temp = buffer[j];
                buffer[j] = buffer[swapPos];
                buffer[swapPos] = temp;
            }

            for (int N = 2; N <= buffer.Length; N <<= 1)
            {
                for (int i = 0; i < buffer.Length; i += N)
                {
                    for (int k = 0; k < N / 2; k++)
                    {
                        int evenIndex = i + k;
                        int oddIndex = i + k + (N / 2);
                        var even = buffer[evenIndex];
                        var odd = buffer[oddIndex];

                        double term = -2 * Math.PI * k / (double) N;
                        Complex exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;

                        buffer[evenIndex] = even + exp;
                        buffer[oddIndex] = even - exp;
                    }
                }
            }
        }

        // aSamples.Length need to be a power of two
        public static Complex[] CalculateFFT(Complex[] aSamples)
        {
            int power = (int) Math.Log(aSamples.Length, 2);
            int count = 1;
            for (int i = 0; i < power; i++)
                count <<= 1;

            int mid = count >> 1; // mid = count / 2;
            int j = 0;
            for (int i = 0; i < count - 1; i++)
            {
                if (i < j)
                {
                    var tmp = aSamples[i];
                    aSamples[i] = aSamples[j];
                    aSamples[j] = tmp;
                }

                int k = mid;
                while (k <= j)
                {
                    j -= k;
                    k >>= 1;
                }

                j += k;
            }

            Complex r = new Complex(-1, 0);
            int l2 = 1;
            for (int l = 0; l < power; l++)
            {
                int l1 = l2;
                l2 <<= 1;
                Complex r2 = new Complex(1, 0);
                for (int n = 0; n < l1; n++)
                {
                    for (int i = n; i < count; i += l2)
                    {
                        int i1 = i + l1;
                        Complex tmp = r2 * aSamples[i1];
                        aSamples[i1] = aSamples[i] - tmp;
                        aSamples[i] += tmp;
                    }

                    r2 = r2 * r;
                }

                r.Im = Math.Sqrt((1d - r.Re) / 2d);
                r.Im = -r.Im;
                r.Re = Math.Sqrt((1d + r.Re) / 2d);
            }

            //double scale = 1d / count;
            //for (int i = 0; i < count; i++)
            //    aSamples[i] *= scale;
            return aSamples;
        }
    }
}