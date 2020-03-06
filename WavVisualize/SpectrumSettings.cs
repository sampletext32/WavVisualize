namespace WavVisualize
{
    public class SpectrumSettings
    {
        //высота цифровых кусочков
        public int DigitalPieceHeight { get; }

        //расстояние между цифровыми кусочками
        public int DistanceBetweenBands { get; }

        //нужно ли рисовать цифровые столбики
        public bool DisplayDigital { get; }

        //количество столбиков спектра
        public int TotalSpectrumBands { get; }

        public SpectrumSettings(int digitalPieceHeight, int distanceBetweenBands, bool displayDigital,
            int totalSpectrumBands)
        {
            DigitalPieceHeight = digitalPieceHeight;
            DistanceBetweenBands = distanceBetweenBands;
            DisplayDigital = displayDigital;
            TotalSpectrumBands = totalSpectrumBands;
        }
    }
}