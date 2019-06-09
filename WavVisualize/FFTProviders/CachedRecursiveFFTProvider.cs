using System;

namespace WavVisualize
{
    public class CachedRecursiveFFTProvider : FFTProvider
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
                    cache[i] = w(i, samples);
                }
                _cacheLevels[base2] = cache;
                base2--;
                samples /= 2;
            }
        }

        private static Complex w(int x, int n)
        {
            if (x % n == 0) return (Complex)1;
            double arg = -2 * Math.PI * x / n;
            return new Complex(Math.Cos(arg), Math.Sin(arg)); //преобразование комплексного экспонентного вида в обычный
        }

        public CachedRecursiveFFTProvider(int samples, bool applyTimeThinning) : base(samples, applyTimeThinning)
        {
            InitAllCache();
        }

        protected override void Algorithm()
        {
            ValuesBuffer = FFT(ValuesBuffer, Samples);
        }

        private Complex cachedW(int x, int n)
        {
            int base2 = FastPowLog2Provider.FastLog2(n);
            return _cacheLevels[base2][x];
        }

        private Complex[] FFT(Complex[] values, int length)
        {
            Complex[] X;
            if (length == 2)
            {
                X = new Complex[2];
                X[0] = values[0] + values[1];
                X[1] = values[0] - values[1];
            }
            else
            {
                Complex[] x_even = new Complex[length / 2];
                Complex[] x_odd = new Complex[length / 2];
                for (int i = 0; i < length / 2; i++)
                {
                    x_even[i] = values[2 * i];
                    x_odd[i] = values[2 * i + 1];
                }

                X = new Complex[length];
                Complex[] X_even = new Complex[length / 2];
                Complex[] X_odd = new Complex[length / 2];

                //Создаём 2 задачи, т.к. вычисления могут выполняться параллельно
                Action[] tasks =
                {
                    () => { X_even = FFT(x_even, length / 2); },
                    () => { X_odd = FFT(x_odd, length / 2); }
                };

                //if (parallel)
                //{
                //    Parallel.ForEach(tasks, action => action.Invoke());
                //}
                //else
                //{
                //    
                //}
                Array.ForEach(tasks, action => action.Invoke());
                for (int i = 0; i < length / 2; i++)
                {
                    Complex rotationAbsMultipliedByValue = cachedW(i, length) * X_odd[i];
                    X[i] = X_even[i] + rotationAbsMultipliedByValue;
                    X[i + length / 2] = X_even[i] - rotationAbsMultipliedByValue;
                }
            }

            return X;
        }
    }
}