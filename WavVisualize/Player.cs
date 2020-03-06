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

        private SpectrumDiagram _spectrumDiagram;
        
        private Settings _settings;

        public void OnUpdate()
        {

        }

        public void OnDraw(Graphics waveformGraphics, Graphics spectrumDiagramGraphics, Graphics volumeGraphics, Graphics spectrumGraphics)
        {
            //Draw Waveform
            //Draw Spectrum Diagram
            //Draw Volume
            //Draw Spectrum
        }

        public Player(Settings settings)
        {
            _settings = settings;
        }
    }
}