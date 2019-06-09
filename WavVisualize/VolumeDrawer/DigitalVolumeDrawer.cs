using System.Drawing;

namespace WavVisualize
{
    public class DigitalVolumeDrawer : VolumeDrawer
    {
        private int _bandPieces;
        private float _bandPieceHeight;
        private float _distanceBetweenPieces;

        public DigitalVolumeDrawer(Rectangle displayRectangle, Color colorL, Color colorR,
            int bandPieces, float distanceBetweenPieces) : base(displayRectangle, colorL, colorR)
        {
            _bandPieces = bandPieces;
            _distanceBetweenPieces = distanceBetweenPieces;

            //высота одного блока = общей высоте минус общая высота всех расстояний между блоками
            _bandPieceHeight = (DisplayRectangle.Height - (bandPieces - 1) * distanceBetweenPieces) / _bandPieces;
        }

        public override void Draw(Graphics g)
        {
            //вычисляем количество цифровых кусочков = громкость * общее количество кусочков
            int digitalPartsL = (int) (NormalizedVolumeL * _bandPieces);
            int digitalPartsR = (int) (NormalizedVolumeR * _bandPieces);

            //рисуем цифровые части левой громкости
            for (int i = 0; i < digitalPartsL; i++)
            {
                g.FillRectangle(LeftBrush, DisplayRectangle.Left,
                    DisplayRectangle.Bottom - i * (_bandPieceHeight + _distanceBetweenPieces) - _bandPieceHeight,
                    DisplayRectangle.CenterW, _bandPieceHeight);
            }

            //рисуем цифровые части правой громкости
            for (int i = 0; i < digitalPartsR; i++)
            {
                g.FillRectangle(RightBrush, DisplayRectangle.CenterW,
                    DisplayRectangle.Bottom - i * (_bandPieceHeight + _distanceBetweenPieces) - _bandPieceHeight,
                    DisplayRectangle.CenterW, _bandPieceHeight);
            }
        }
    }
}