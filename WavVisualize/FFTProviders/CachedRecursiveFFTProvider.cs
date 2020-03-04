using System;

namespace WavVisualize
{
    public class CachedRecursiveFFTProvider : ClassicRecursiveFFTProvider
    {
        private Complex[][] _cacheLevels;

        private void InitAllCache()
        {
            int base2 = FastPowLog2Provider.FastLog2(Samples);
            _cacheLevels = new Complex[base2 + 1][];
            int samples = Samples;
            while (base2 > 0)
            {
                Complex[] cache = new Complex[samples];
                for (int i = 0; i < samples; i++)
                {
                    cache[i] = W(i, samples);
                }
                _cacheLevels[base2] = cache;
                base2--;
                samples /= 2;
            }
        }

        public CachedRecursiveFFTProvider(int samples, bool applyTimeThinning) : base(samples, applyTimeThinning)
        {
            InitAllCache();
        }

        protected override void Algorithm()
        {
            ValuesBuffer = FFT(ValuesBuffer, Samples);
        }

        protected override Complex W(int x, int n)
        {
            int base2 = FastPowLog2Provider.FastLog2(n);
            return _cacheLevels[base2][x];
        }

    }
}