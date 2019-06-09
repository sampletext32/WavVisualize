using System.Drawing;

namespace WavVisualize
{
    public abstract class BandBasedSpectrumDrawer : SpectrumDrawer
    {
        protected int BandsCount;
        protected float BandWidth;
        protected float DistanceBetweenBands;

        public BandBasedSpectrumDrawer(
            int spectrumSamples,
            float constantHeightMultiplier,
            Rectangle displayRectangle,
            Color color,
            int bandsCount, float distanceBetweenBands) :
            base(spectrumSamples, constantHeightMultiplier, displayRectangle, color)
        {
            BandsCount = bandsCount;
            DistanceBetweenBands = distanceBetweenBands;
            
            BandWidth = (DisplayRectangle.Width - (BandsCount - 1) * DistanceBetweenBands) / BandsCount;
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

                float analogValue = SpectrumValues[i] * Log10Normalizing(i) * DisplayRectangle.Height * ConstantHeightMultiplier;
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