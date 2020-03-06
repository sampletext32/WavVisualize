using System;

namespace WavVisualize
{
    public class WaveformSettings
    {
        //создавать ли волну последовательно
        public bool CreateWaveformSequentially { get; }

        //количество потоков для создания волны (-1, чтобы главный поток рисовал без зависаний)
        public int ThreadsForWaveformCreation { get; }

        //частота пропуска сэмплов при создании волны
        public int WaveformSkipSampleRate { get; }

        //масштаб
        public float ScaleX { get; }

        public WaveformSettings(bool createWaveformSequentially, int threadsForWaveformCreation,
            int waveformSkipSampleRate, float scaleX)
        {
            CreateWaveformSequentially = createWaveformSequentially;
            ThreadsForWaveformCreation = threadsForWaveformCreation;
            WaveformSkipSampleRate = waveformSkipSampleRate;
            ScaleX = scaleX;
        }
    }
}