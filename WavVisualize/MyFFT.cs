using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class MyFFT
    {
        private static bool parallel = false;

        /// <summary>
        /// Вычисление поворачивающего модуля e^(-i*2*PI*x/N)
        /// </summary>
        private static Complex w(int x, int n)
        {
            if (x % n == 0) return (Complex) 1;
            double arg = -2 * Math.PI * x / n;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }

        public static float[] FFT(float[] values, int start, int length)
        {
            Complex[] complexValues = new Complex[length];
            for (int i = 0; i < length; i++)
            {
                complexValues[i] = new Complex(values[start + i], 0);
            }

            complexValues = fft(complexValues, 0, length);
            float[] retVal = new float[length];
            for (int i = 0; i < length; i++)
            {
                retVal[i] = (float) Math.Sqrt(complexValues[i].Re * complexValues[i].Re +
                                              complexValues[i].Im * complexValues[i].Im) / length / 2;
            }

            return retVal;
        }

        private static List<Task> tasks = new List<Task>();

        /// <summary>
        /// Возвращает спектр сигнала
        /// </summary>
        /// <param name="x">Массив значений сигнала. Количество значений должно быть степенью 2</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static Complex[] fft(Complex[] x, int start, int length)
        {
            Complex[] X;
            if (length == 2)
            {
                X = new Complex[2];
                X[start + 0] = x[start + 0] + x[start + 1];
                X[start + 1] = x[start + 0] - x[start + 1];
            }
            else
            {
                Complex[] x_even = new Complex[length / 2];
                Complex[] x_odd = new Complex[length / 2];
                for (int i = 0; i < length / 2; i++)
                {
                    x_even[i] = x[start + 2 * i];
                    x_odd[i] = x[start + 2 * i + 1];
                }

                X = new Complex[length];
                Complex[] X_even = new Complex[length / 2];
                Complex[] X_odd = new Complex[length / 2];

                Action[] tasks =
                {
                    () => { X_even = fft(x_even, 0, length / 2); },
                    () => { X_odd = fft(x_odd, 0, length / 2); }
                };
                if (parallel)
                {
                    Parallel.ForEach(tasks, action => action.Invoke());
                }
                else
                {
                    Array.ForEach(tasks, action => action.Invoke());
                }

                for (int i = 0; i < length / 2; i++)
                {
                    Complex rotationAbsMultipliedByValue = w(i, length) * X_odd[i];
                    X[i] = X_even[i] + rotationAbsMultipliedByValue;
                    X[i + length / 2] = X_even[i] - rotationAbsMultipliedByValue;
                }
            }

            return X;
        }

        /// <summary>
        /// Центровка массива значений полученных в fft (спектральная составляющая при нулевой частоте будет в центре массива)
        /// </summary>
        /// <param name="X">Массив значений полученный в fft</param>
        /// <returns></returns>
        public static Complex[] nfft(Complex[] X)
        {
            int N = X.Length;
            Complex[] X_n = new Complex[N];
            for (int i = 0; i < N / 2; i++)
            {
                X_n[i] = X[N / 2 + i];
                X_n[N / 2 + i] = X[i];
            }

            return X_n;
        }
    }
}