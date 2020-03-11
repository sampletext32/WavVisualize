using System;

namespace WavVisualize
{
    public abstract class FFTProvider
    {
        private static float Ln10 = (float)Math.Log(10);
        protected float[] Frequencies;
        protected Complex[] ValuesBuffer;
        protected int Samples;
        private bool _applyTimeThinning;

        protected abstract void Algorithm();

        public void Calculate(float[] values, int start)
        {
            CopyValues(values, start);

            Algorithm();

            TakeValues();
        }

        private void CopyValues(float[] values, int start)
        {
            if (_applyTimeThinning)
            {
                for (int i = 0; i < Samples; i++)
                {
                    if (i % 2 == 0)
                    {
                        ValuesBuffer[i].Re = values[start + i / 2];
                    }
                    else
                    {
                        ValuesBuffer[i].Re = 0;
                    }

                    ValuesBuffer[i].Im = 0;
                }
            }
            else
            {
                for (int i = 0; i < Samples; i++)
                {
                    ValuesBuffer[i].Re = values[start + i];
                    ValuesBuffer[i].Im = 0;
                }
            }
        }

        private void TakeValues()
        {
            if (_applyTimeThinning)
            {
                for (int i = 0; i < Samples; i++)
                {
                    Frequencies[i] = (float) ValuesBuffer[i].Magnitude / (Samples / 2f) * Ln10;
                }
            }
            else
            {
                for (int i = 0; i < Samples; i++)
                {
                    //Все комплексные частоты переводим в числа, используя модуль комплексного числа.
                    //Нормализуем частоты деля все значения на количество сигналов
                    //Дополнительно делим на 2, т.к. только половина выходных сигналов действительно является искомыми частотами
                    Frequencies[i] = (float) ValuesBuffer[i].Magnitude / (Samples) * Ln10;
                }
            }
        }

        public float[] Get()
        {
            return Frequencies;
        }

        protected FFTProvider(int samples, bool applyTimeThinning)
        {
            Samples = samples;
            _applyTimeThinning = applyTimeThinning;
            Frequencies = new float[Samples];
            ValuesBuffer = new Complex[Samples];
        }
    }
}