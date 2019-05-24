using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;


namespace WavVisualize
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        WindowsMediaPlayer wmp = new WindowsMediaPlayer();

        private WavFileData currentWavFileData;

        private void pictureBoxPlot_Paint(object sender, PaintEventArgs e)
        {
            if (currentWavFileData?.WaveformBitmap != null)
            {
                e.Graphics.DrawImage(currentWavFileData.WaveformBitmap, 0, 0, pictureBoxPlot.Width,
                    pictureBoxPlot.Height);
            }

            e.Graphics.FillRectangle(Brushes.Black, playerPositionNormalized * pictureBoxPlot.Width, 0, 1,
                pictureBoxPlot.Height);
        }

        private float playerPositionNormalized = 0f;

        private void timerUpdater_Tick(object sender, EventArgs e)
        {
            int h = (int) wmp.controls.currentPosition / 3600 % 3600;
            int m = (int) wmp.controls.currentPosition / 60 % 60;
            int s = (int) wmp.controls.currentPosition % 60;

            int h1 = (int) wmp.currentMedia.duration / 3600 % 3600;
            int m1 = (int) wmp.currentMedia.duration / 60 % 60;
            int s1 = (int) wmp.currentMedia.duration % 60;
            this.Text = $@"{h:00} : {m:00} : {s:00} / {h1:00} : {m1:00} : {s1:00}";
            playerPositionNormalized = (float) (wmp.controls.currentPosition / wmp.currentMedia.duration);
            pictureBoxPlot.Invalidate();
            pictureBoxSpectrum.Invalidate();
        }

        float _currentVolumeL;
        float _currentVolumeR;

        const int framesPerSecond = 30;
        const float easingCoef = 0.9f;
        const int bandWidth = 20;
        const int digitalBandHeight = 10;
        const int distanceBetweenBands = 2;
        float spectrumHeight = 0;

        float spectrumBaselineY = 0;

        int digitalBandsCount = 15;
        int totalSpectrumBands = 100;
        int totalSpectrumWidth = 10;

        private void GetMaxVolume(float[] left, float[] right, ref float lMax, ref float rMax, int start, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (left[start + i] > lMax)
                {
                    lMax = left[start + i];
                }

                if (right[start + i] > rMax)
                {
                    rMax = right[start + i];
                }
            }
        }

        private void pictureBoxSpectrum_Paint(object sender, PaintEventArgs e)
        {
            if (wmp.playState == WMPPlayState.wmppsPlaying)
            {
                int position = (int) (playerPositionNormalized * currentWavFileData.SamplesCount);

                int pieceLength = currentWavFileData.SampleRate / framesPerSecond;
                if (position < pieceLength / 2) return;

                int start = (position / pieceLength + 1) * pieceLength;
                int end = Math.Min(currentWavFileData.SamplesCount, (position / pieceLength + 2) * pieceLength);

                int length = end - start;
                if (length % 2 == 1) length++;

                float maxL = 0;
                float maxR = 0;

                GetMaxVolume(currentWavFileData.LeftChannel, currentWavFileData.RightChannel, ref maxL, ref maxR, start,
                    length);

                _currentVolumeL += (maxL - _currentVolumeL) * easingCoef;
                _currentVolumeR += (maxR - _currentVolumeR) * easingCoef;

                int digitalPartsL = (int) (_currentVolumeL * digitalBandsCount);
                int digitalPartsR = (int) (_currentVolumeR * digitalBandsCount);

                e.Graphics.DrawLine(Pens.LawnGreen, 0, spectrumBaselineY - _currentVolumeL * pictureBoxSpectrum.Height / 2, bandWidth,
                    spectrumBaselineY - _currentVolumeL * pictureBoxSpectrum.Height / 2);


                e.Graphics.DrawLine(Pens.Red, bandWidth, spectrumBaselineY - _currentVolumeR * pictureBoxSpectrum.Height / 2,
                    bandWidth + bandWidth,
                    spectrumBaselineY - _currentVolumeR * pictureBoxSpectrum.Height / 2);

                for (int i = 1; i < digitalPartsL + 1; i++)
                {
                    e.Graphics.FillRectangle(Brushes.LawnGreen, 0,
                        spectrumBaselineY - i * (digitalBandHeight + distanceBetweenBands), bandWidth,
                        digitalBandHeight);
                }

                for (int i = 1; i < digitalPartsR + 1; i++)
                {
                    e.Graphics.FillRectangle(Brushes.Red, bandWidth,
                        spectrumBaselineY - i * (digitalBandHeight + distanceBetweenBands), bandWidth,
                        digitalBandHeight);
                }

                float[] spectrum = currentWavFileData.GetSpectrumForPosition(playerPositionNormalized);

                DrawSpectrumBeauty(e.Graphics, spectrum);
                //DrawSpectrumOriginal(e.Graphics, spectrum);
            }
        }

        private void DrawSpectrumOriginal(Graphics g, float[] spectrum)
        {
            int useLength = spectrum.Length / 2;
            float sbandWidth = (float)totalSpectrumWidth / useLength;
            for (int i = 0; i < useLength; i++)
            {
                float displayingHeight = Math.Abs(spectrumHeight * spectrum[i] * 2);
                
                g.FillRectangle(Brushes.Red,
                    bandWidth + bandWidth + 50 + i * sbandWidth,
                    spectrumBaselineY - displayingHeight,
                    sbandWidth, displayingHeight);
            }
        }

        private void DrawSpectrumBeauty(Graphics g, float[] spectrum)
        {
            int useLength = spectrum.Length / 2;
            int valuesPerBand = (int) ((float) useLength / totalSpectrumBands);
            float bandWidth = (float) totalSpectrumWidth / totalSpectrumBands;

            for (int i = 0; i < totalSpectrumBands; i++)
            {
                float sumVal = 0f;
                for (int j = 0; j < valuesPerBand; j++)
                {
                    //if (spectrum[i * valuesPerBand + j] > maxVal) maxVal = spectrum[i * valuesPerBand + j];
                    sumVal += spectrum[i * valuesPerBand + j];
                }

                sumVal /= valuesPerBand;
                sumVal *= 10f;
                g.FillRectangle(Brushes.Red,
                    bandWidth + bandWidth + 50 + bandWidth * i,
                    spectrumBaselineY - Math.Abs(spectrumHeight * sumVal),
                    bandWidth, Math.Abs(spectrumHeight * sumVal));
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            
        }

        private void pictureBoxPlot_MouseDown(object sender, MouseEventArgs e)
        {
            wmp.controls.currentPosition = ((float) e.X / pictureBoxSpectrum.Width) * wmp.currentMedia.duration;
            wmp.controls.play();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Файлы WAV (*.wav)|*.wav";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                //string filename = "clear.wav";
                //string filename = "elements.wav";
                //string filename = "divinorum.wav";
                //string filename = "energize.wav";
                string filename = opf.FileName;

                currentWavFileData = WavFileData.ReadWav(filename);

                spectrumHeight = pictureBoxSpectrum.Height;
                totalSpectrumWidth = pictureBoxSpectrum.Width / 2;

                spectrumBaselineY = pictureBoxSpectrum.Height;

                Task.Run(() =>
                {
                    currentWavFileData.RecreateWaveformBitmap(pictureBoxPlot.Width, pictureBoxPlot.Height);
                });

                wmp.currentMedia = wmp.newMedia(filename);
                wmp.controls.play();
                digitalBandsCount = pictureBoxSpectrum.Height / (digitalBandHeight + distanceBetweenBands);

                timerUpdater.Start();

                this.BringToFront();
            }
            else
            {
                Application.Exit();
            }
        }
    }
}