using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class DigitalBandSpectrumDrawer : BandBasedSpectrumDrawer
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
                g.FillRectangle(Brush, Left + band * (BandWidth + DistanceBetweenBands),
                    Bottom - k * (BandPieceHeight + DistanceBetweenPieces) - BandPieceHeight,
                    BandWidth,
                    BandPieceHeight);
            }
        }

        public DigitalBandSpectrumDrawer(int spectrumSamples, float constantHeightMultiplier, float left, float right, float top, float bottom, Color color, int bandsCount, float distanceBetweenBands, int bandPieces, float distanceBetweenPieces) : base(spectrumSamples, constantHeightMultiplier, left, right, top, bottom, color, bandsCount, distanceBetweenBands)
        {
            BandPieces = bandPieces;
            DistanceBetweenPieces = distanceBetweenPieces;

            //высота одного блока = общей высоте минус общая высота всех расстояний между блоками
            BandPieceHeight = (Height - (bandPieces - 1) * distanceBetweenPieces) / BandPieces;
        }
    }
}
