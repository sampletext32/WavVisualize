using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class DigitalVolumeDrawer : VolumeDrawer
    {
        private int _bandPieces;
        private float _bandPieceHeight;
        private float _distanceBetweenPieces;

        public DigitalVolumeDrawer(float left, float right, float top, float bottom, Color colorL, Color colorR,
            int bandPieces, float distanceBetweenPieces) : base(left, right, top, bottom, colorL, colorR)
        {
            _bandPieces = bandPieces;
            _distanceBetweenPieces = distanceBetweenPieces;

            //высота одного блока = общей высоте минус общая высота всех расстояний между блоками
            _bandPieceHeight = (Height - (bandPieces - 1) * distanceBetweenPieces) / _bandPieces;
        }

        public override void Draw(Graphics g)
        {
            //вычисляем количество цифровых кусочков = громкость * общее количество кусочков
            int digitalPartsL = (int) (NormalizedVolumeL * _bandPieces);
            int digitalPartsR = (int) (NormalizedVolumeR * _bandPieces);

            //рисуем цифровые части левой громкости
            for (int i = 0; i < digitalPartsL; i++)
            {
                g.FillRectangle(LeftBrush, 0,
                    Bottom - i * (_bandPieceHeight + _distanceBetweenPieces) - _bandPieceHeight, Width / 2, _bandPieceHeight);
            }

            //рисуем цифровые части правой громкости
            for (int i = 0; i < digitalPartsR; i++)
            {
                g.FillRectangle(RightBrush, Width / 2,
                    Bottom - i * (_bandPieceHeight + _distanceBetweenPieces) - _bandPieceHeight, Width / 2, _bandPieceHeight);
            }
        }
    }
}