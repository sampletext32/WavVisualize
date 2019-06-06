using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    abstract class BandBasedSpectrumDrawer : SpectrumDrawer
    {
        protected int BandsCount;
        protected float BandWidth;
        protected float DistanceBetweenBands;

        public BandBasedSpectrumDrawer(
            int spectrumSamples,
            float constantHeightMultiplier,
            float left, float right, float top, float bottom,
            Color color,
            int bandsCount, float distanceBetweenBands) :
            base(spectrumSamples, constantHeightMultiplier, left, right, top, bottom, color)
        {
            BandsCount = bandsCount;
            DistanceBetweenBands = distanceBetweenBands;
            
            BandWidth = (Width - (BandsCount - 1) * DistanceBetweenBands) / BandsCount;
        }

        protected override void InnerDraw(Graphics g, int useLength)
        {
            int lastBand = -1; //последний активный столбик
            float maxInLastBand = 0f; //максимальное значение частоты в последнем столбике

            for (int i = 0; i < useLength; i++)
            {
                int band = (int) ((float) i / useLength * BandsCount);

                if (band > lastBand) //если сменился столбик
                {
                    //обнуляем показатель
                    lastBand = band;
                    maxInLastBand = 0f;
                }

                float analogValue = SpectrumValues[i] * Log10Normalizing(i) * Height * ConstantHeightMultiplier;
                if (analogValue > maxInLastBand)
                {
                    maxInLastBand = analogValue; //пересохраняем частоту

                    DrawBand(g, band, analogValue);
                }
            }
        }

        protected abstract void DrawBand(Graphics g, int band, float value);
    }
}