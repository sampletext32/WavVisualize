namespace WavVisualize
{
    public class VolumeSettings
    {
        //высота цифровых кусочков
        public int DigitalPieceHeight { get; }

        //расстояние между кусочками
        public int DistanceBetweenPieces { get; }

        public VolumeSettings(int digitalPieceHeight, int distanceBetweenPieces)
        {
            DigitalPieceHeight = digitalPieceHeight;
            DistanceBetweenPieces = distanceBetweenPieces;
        }
    }
}