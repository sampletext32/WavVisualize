using System.Drawing;

namespace WavVisualize
{
    public abstract class SpectrumDiagramDrawer
    {
        protected int SpectrumSamples;
        protected float[] SpectrumValues;

        protected Rectangle DisplayRectangle;

        protected FFTProvider FftProvider;
        protected WavFileData FileData;

        protected DirectBitmap Diagram;
        protected Color Color;

        protected int TrimmingFrequency;
        protected bool ApplyTimeThinning;

        protected bool Canceled;

        public void SetTrimmingFrequency(int frequency)
        {
            TrimmingFrequency = frequency;
        }

        public void SetApplyTimeThinning(bool apply)
        {
            ApplyTimeThinning = apply;
            FftProvider = new CorrectCooleyTukeyInPlaceFFTProvider(SpectrumSamples, ApplyTimeThinning);
        }

        protected int IntensityToArgb(float intensity)
        {
            int lowEnd = 10;
            int alpha = (int) (lowEnd + intensity * (byte.MaxValue - lowEnd));
            int red = Color.R;
            int green = Color.G;
            int blue = Color.B;
            int argb = (int) ((uint) (red << 16 | green << 8 | blue | alpha << 24));
            return argb;
        }

        public abstract void Draw(Graphics g);

        public abstract void Recreate();

        public void Cancel()
        {
            Canceled = true;
        }

        public SpectrumDiagramDrawer(int spectrumSamples, Rectangle displayRectangle, WavFileData fileData)
        {
            SpectrumSamples = spectrumSamples;
            SpectrumValues = new float[SpectrumSamples];
            DisplayRectangle = displayRectangle;
            Diagram = new DirectBitmap((int) displayRectangle.Width, (int) displayRectangle.Height);
            Color = Color.OrangeRed;

            FftProvider = new CorrectCooleyTukeyInPlaceFFTProvider(SpectrumSamples, ApplyTimeThinning);
            FileData = fileData;
        }

        ~SpectrumDiagramDrawer()
        {
            Diagram.Dispose();
        }
    }
}