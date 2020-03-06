namespace WavVisualize
{
    public class Player
    {
        private WindowsMediaPlayerProvider _playerProvider;

        private WavFileData _currentWavFileData;

        private WaveformProvider _waveformProvider;

        private FFTProvider _fftProvider;

        private VolumeProvider _volumeProvider;

        private VolumeDrawer _volumeDrawer;

        private SpectrumDrawer _spectrumDrawer;

        private SpectrumDiagram _spectrumDiagram;
        
        private Settings _settings;

        public Player(Settings settings)
        {
            _settings = settings;
        }
    }
}