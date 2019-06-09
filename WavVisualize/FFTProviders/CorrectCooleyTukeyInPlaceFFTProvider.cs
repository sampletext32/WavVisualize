using System;

namespace WavVisualize
{
    public class CorrectCooleyTukeyInPlaceFFTProvider : FFTProvider
    {
        public CorrectCooleyTukeyInPlaceFFTProvider(int samples, bool applyTimeThinning) : base(samples, applyTimeThinning)
        {
            r = new Complex(0, 0);
            r2 = new Complex(0, 0);
        }

        private Complex r, r2;

        protected override void Algorithm()
        {
            int power = FastPowLog2Provider.FastLog2(Samples);
            
            int mid = Samples >> 1;
            int j = 0;
            for (int i = 0; i < Samples - 1; i++)
            {
                if (i < j)
                {
                    var tmp = ValuesBuffer[i];
                    ValuesBuffer[i] = ValuesBuffer[j];
                    ValuesBuffer[j] = tmp;
                }

                int k = mid;
                while (k <= j)
                {
                    j -= k;
                    k >>= 1;
                }

                j += k;
            }

            r.Re = -1;
            r.Im = 0;

            int l2 = 1;
            for (int l = 0; l < power; l++)
            {
                int l1 = l2;
                l2 <<= 1;

                r2.Re = 1;
                r2.Im = 0;

                for (int n = 0; n < l1; n++)
                {
                    for (int i = n; i < Samples; i += l2)
                    {
                        int i1 = i + l1;
                        Complex tmp = r2 * ValuesBuffer[i1];
                        ValuesBuffer[i1] = ValuesBuffer[i] - tmp;
                        ValuesBuffer[i] += tmp;
                    }

                    r2 = r2 * r;
                }

                r.Im = -Math.Sqrt((1 - r.Re) / 2d);
                r.Re = Math.Sqrt((1 + r.Re) / 2d);
            }
        }
    }
}