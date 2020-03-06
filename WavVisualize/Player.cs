using System.Drawing;

namespace WavVisualize
{
    public class Player
    {
        private WindowsMediaPlayerProvider _playerProvider;

        private WavFileData _currentWavFileData;

        private WaveformProvider _waveformProvider;

        private VolumeProvider _volumeProvider;

        private FFTProvider _fftProvider;

        private VolumeDrawer _volumeDrawer;

        private SpectrumDrawer _spectrumDrawer;

        private SpectrumDiagramDrawer _spectrumDiagramDrawer;

        private Settings _settings;

        public void OnUpdate()
        {
        }

        public void OnDraw(DirectBitmap waveformBitmap, DirectBitmap spectrumDiagramBitmap, DirectBitmap volumeBitmap,
            DirectBitmap spectrumBitmap)
        {
            //Draw Waveform
            //Draw Spectrum Diagram
            //Draw Volume
            //Draw Spectrum
        }

        private void InitializeWaveformProvider()
        {
        }

        private void InitializeVolumeProvider()
        {
        }

        private void InitializeFftProvider()
        {
        }

        private void InitializeVolumeDrawer()
        {
        }

        private void InitializeSpectrumDrawer()
        {
        }

        private void InitializeSpectrumDiagramDrawer()
        {
        }

        public Player(Settings settings)
        {
            _settings = settings;
            InitializeWaveformProvider();
            InitializeVolumeProvider();
            InitializeFftProvider();
            InitializeVolumeDrawer();
            InitializeSpectrumDrawer();
            InitializeSpectrumDiagramDrawer();
            
        }
    }
}