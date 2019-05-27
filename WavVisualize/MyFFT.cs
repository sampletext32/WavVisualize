using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WavVisualize.NewFFT;

namespace WavVisualize
{
    //Класс отвечающий за преобразование Фурье
    public class MyFFT
    {
        //Нужно ли производить параллельные вычисления
        private static bool parallel = false;

        public static bool useCache = true;

        //private static Dictionary<int, Complex[]> _cacheLevels = new Dictionary<int, Complex[]>();

        private static Complex[][] _cacheLevels;

        public static void InitAllCache(int samples)
        {
            int base2 = (int) Math.Log(samples, 2);
            _cacheLevels = new Complex[base2 + 1][];
            while (samples > 3)
            {
                InitCache(samples);

                samples /= 2;
            }
        }

        public static void InitCache(int samples)
        {
            int base2 = (int) Math.Log(samples, 2);

            Complex[] cache = new Complex[samples];
            for (int i = 0; i < samples; i++)
            {
                cache[i] = w(i, samples);
            }

            _cacheLevels[base2] = cache;
        }

        /// <summary>
        /// <para>Вычисление поворачивающего модуля e^(-i*2*PI*x/n)</para>
        /// <para>x - индекс сигнала</para>
        /// <para>n - количество сигналов</para>
        /// </summary>
        private static Complex w(int x, int n)
        {
            if (x % n == 0) return (Complex) 1;
            double arg = -2 * Math.PI * x / n;
            return new Complex(Math.Cos(arg), Math.Sin(arg)); //преобразование комплексного экспонентного вида в обычный
        }

        private static Complex cachedW(int x, int n)
        {
            int base2 = (int) Math.Log(n, 2);
            return _cacheLevels[base2][x];
        }

        //Функция производит преобразование Фурье над массивом сигналов values начиная со [start] и используя length сигналов
        //Здесь length должно быть 2^n
        public static float[] FFT(float[] values, int start, int length)
        {
            Complex[] complexValues = new Complex[length]; //создаём пустой массив комплексных чисел
            for (int i = 0; i < length; i++)
            {
                complexValues[i] =
                    new Complex(values[start + i],
                        0); //заполняем так, чтобы действительная часть была сигналом, а мнимая нулём
            }

            //complexValues = fft(complexValues, 0, length); //производим преобразование Фурье
            //MyNewFFT.FFT(complexValues);

            complexValues = MyNewFFT.CalculateFFT(complexValues);

            //inPlace_nfft(complexValues);

            float[] frequencies = new float[length]; //создаём массив частот
            for (int i = 0; i < length; i++)
            {
                //Все комплексные частоты переводим в числа, используя модуль комплексного числа.
                //Нормализуем частоты деля все значения на количество сигналов
                //Дополнительно делим на 2, т.к. только половина выходных сигналов действительно является искомыми частотами
                frequencies[i] = (float) complexValues[i].Re / length;
            }

            return frequencies;
        }

        /// <summary>
        /// Непосредственное преобразование Фурье
        /// </summary>
        /// <param name="x">Массив комплексных сигналов</param>
        /// <param name="start">Индекс начала</param>
        /// <param name="length">Количество сигналов</param>
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

                //Создаём 2 задачи, т.к. вычисления могут выполняться параллельно
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
                    Complex rotationAbsMultipliedByValue = (useCache ? cachedW(i, length) : w(i, length)) * X_odd[i];
                    X[i] = X_even[i] + rotationAbsMultipliedByValue;
                    X[i + length / 2] = X_even[i] - rotationAbsMultipliedByValue;
                }
            }

            return X;
        }

        /// <summary>
        /// <para>Центрирование значений полученных из преобразования Фурье </para>
        /// <para>Низкие частоты становятся по-середине</para>
        /// </summary>
        /// <param name="X">Массив сигналов</param>
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

        public static void inPlace_nfft(Complex[] X)
        {
            int N = X.Length;
            for (int i = 0; i < N / 2; i++)
            {
                Complex val = X[i];

                X[i] = X[N / 2 + i];
                X[N / 2 + i] = val;
            }
        }
    }
}