using System.Drawing;

namespace WavVisualize
{
    public class DigitalBandSpectrumDrawer : BandBasedSpectrumDrawer
    {
        protected int BandPieces;
        protected float BandPieceHeight;
        protected float DistanceBetweenPieces;

        protected override void DrawBand(Graphics g, int band, float value)
        {
            int digitalParts = (int)(value / (BandPieceHeight + DistanceBetweenPieces));

            //рисуем цифровые части столбика
            for (int k = 0; k < digitalParts; k++)
            {
                g.FillRectangle(Brush, DisplayRectangle.Left + band * (BandWidth + DistanceBetweenBands),
                    DisplayRectangle.Bottom - k * (BandPieceHeight + DistanceBetweenPieces) - BandPieceHeight,
                    BandWidth,
                    BandPieceHeight);
            }
        }

        public DigitalBandSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, Rectangle displayRectangle, Color color, int bandsCount, float distanceBetweenBands, int bandPieces, float distanceBetweenPieces) : base(spectrumSamples, constantHeightMultiplier, displayRectangle, color, bandsCount, distanceBetweenBands)
        {
            BandPieces = bandPieces;
            DistanceBetweenPieces = distanceBetweenPieces;

            //высота одного блока = общей высоте минус общая высота всех расстояний между блоками
            BandPieceHeight = (DisplayRectangle.Height - (bandPieces - 1) * distanceBetweenPieces) / BandPieces;
        }
    }
}
