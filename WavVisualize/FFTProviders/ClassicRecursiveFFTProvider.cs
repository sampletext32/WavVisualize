using System;

namespace WavVisualize
{
    public class ClassicRecursiveFFTProvider : FFTProvider
    {
        public ClassicRecursiveFFTProvider(int samples, bool applyTimeThinning) : base(samples, applyTimeThinning)
        {
        }

        protected override void Algorithm()
        {
            ValuesBuffer = FFT(ValuesBuffer, Samples);
        }

        private static Complex W(int x, int n)
        {
            if (x % n == 0) return (Complex) 1;
            double arg = -2 * Math.PI * x / n;
            return new Complex(Math.Cos(arg), Math.Sin(arg)); //преобразование комплексного экспонентного вида в обычный
        }

        private Complex[] FFT(Complex[] values, int length)
        {
            Complex[] x;
            if (length == 2)
            {
                x = new Complex[2];
                x[0] = values[0] + values[1];
                x[1] = values[0] - values[1];
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

                x = new Complex[length];

                Complex[] X_even = FFT(x_even, length / 2);
                Complex[] X_odd = FFT(x_odd, length / 2);
                
                for (int i = 0; i < length / 2; i++)
                {
                    Complex rotationAbsMultipliedByValue = W(i, length) * X_odd[i];
                    x[i] = X_even[i] + rotationAbsMultipliedByValue;
                    x[i + length / 2] = X_even[i] - rotationAbsMultipliedByValue;
                }
            }

            return x;
        }
    }
}