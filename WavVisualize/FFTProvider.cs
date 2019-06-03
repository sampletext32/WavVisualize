using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public abstract class FFTProvider
    {
        protected float[] Frequencies;
        protected Complex[] ValuesBuffer;

        protected int Samples;
        
        protected abstract void Algorithm();

        public void Calculate(float[] values, int start)
        {
            CopyValues(values, start);

            Algorithm();

            TakeValues();
        }

        private void CopyValues(float[] values, int start)
        {
            for (int i = 0; i < Samples; i++)
            {
                ValuesBuffer[i].Re = values[start + i];
                ValuesBuffer[i].Im = 0;
            }
        }

        private void TakeValues()
        {
            for (int i = 0; i < Samples; i++)
            {
                //Все комплексные частоты переводим в числа, используя модуль комплексного числа.
                //Нормализуем частоты деля все значения на количество сигналов
                //Дополнительно делим на 2, т.к. только половина выходных сигналов действительно является искомыми частотами
                Frequencies[i] = (float) ValuesBuffer[i].Magnitude / Samples / 2;
            }
        }

        public float[] Get()
        {
            return Frequencies;
        }

        protected FFTProvider(int samples)
        {
            Samples = samples;
            Frequencies = new float[Samples];
            ValuesBuffer = new Complex[Samples];
            for (int i = 0; i < Samples; i++)
            {
                ValuesBuffer[i] = new Complex();
            }
        }
    }
}