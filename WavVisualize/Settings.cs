using System;

namespace WavVisualize
{
    public class Settings
    {
        //создавать ли волну последовательно
        public bool CreateWaveformSequentially = true;

        //количество потоков для создания волны (-1, чтобы главный поток рисовал без зависаний)
        public int ThreadsForWaveformCreation = Environment.ProcessorCount - 1;

        //частота пропуска сэмплов при создании волны
        public int WaveformSkipSampleRate = 0;

        //сколько раз в секунду обновляется состояние плеера
        public int UpdateRate = 60;

        //ширина столбиков громкости
        public int VolumeBandWidth = 20;

        //высота цифровых кусочков
        public int DigitalPieceHeight = 2;

        //расстояние между цифровыми кусочками
        public int DistanceBetweenBands = 1;

        //нужно ли рисовать цифровые столбики
        public bool DisplayDigital = true;

        //количество столбиков спектра
        public int TotalSpectrumBands = 100;

        //сколько сэмплов идёт на преобразование спектра (обязательно степень двойки)
        public int SpectrumUseSamples = 4096;

        //применять ли прореживание по времени
        public bool ApplyTimeThinning = true;
    }
}