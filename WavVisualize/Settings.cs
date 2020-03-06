using System;

namespace WavVisualize
{
    public class Settings
    {
        public PlayerSettings PlayerSettings { get; }
        public WaveformSettings WaveformSettings { get; }
        public SpectrumSettings SpectrumSettings { get; }
        public VolumeSettings VolumeSettings { get; }

        public Settings(PlayerSettings playerSettings, WaveformSettings waveformSettings,
            SpectrumSettings spectrumSettings, VolumeSettings volumeSettings)
        {
            PlayerSettings = playerSettings;
            WaveformSettings = waveformSettings;
            SpectrumSettings = spectrumSettings;
            VolumeSettings = volumeSettings;
        }

        public static Settings Default = new Settings(
            new PlayerSettings(60, 4096, 20000, 0.1f, true),
            new WaveformSettings(true, Environment.ProcessorCount - 1, 0, 1),
            new SpectrumSettings(2, 1, false, 100),
            new VolumeSettings(2, 1)
        );
    }
}