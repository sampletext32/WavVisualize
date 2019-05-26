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
using NAudio.Dsp;
using NAudio.Wave;
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
            if (currentWavFileData?.WaveformBitmaps != null)
            {
                if (CreateWaveformSequentially)
                {
                    for (int i = 0; i < ThreadsForWaveformCreation; i++)
                    {
                        e.Graphics.DrawImage(currentWavFileData.WaveformBitmaps[i],
                            i * pictureBoxPlot.Width / ThreadsForWaveformCreation, 0,
                            pictureBoxPlot.Width / ThreadsForWaveformCreation,
                            currentWavFileData.WaveformBitmaps[i].Height);
                    }
                }
                else
                {
                    for (int i = 0; i < ThreadsForWaveformCreation; i++)
                    {
                        e.Graphics.DrawImage(currentWavFileData.WaveformBitmaps[i],
                            pictureBoxPlot.Width / 2f - pictureBoxPlot.Width / 2, 0,
                            pictureBoxPlot.Width,
                            pictureBoxPlot.Height);
                    }
                }
            }

            e.Graphics.FillRectangle(Brushes.Black, playerPositionNormalized * pictureBoxPlot.Width, 0, 1,
                pictureBoxPlot.Height);

            e.Graphics.FillRectangle(Brushes.DarkGray, playerPositionNormalized * pictureBoxPlot.Width - 10,
                pictureBoxPlot.Height - 5, 20, 10);
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

        public float CurrentVolumeL;
        public float CurrentVolumeR;

        public float[] CurrentSpectrum;

        public readonly bool CreateWaveformSequentially = true;
        public readonly int ThreadsForWaveformCreation = 8;
        public readonly int WaveformSkipSampleRate = 0;

        public readonly int FramesPerSecond = 60;
        public float EasingCoef = 0.7f;
        public readonly int BandWidth = 20;
        public readonly int DigitalPieceHeight = 2;
        public readonly int DistanceBetweenBands = 1;
        public float SpectrumHeight;

        public bool DisplayDigital = true;

        public float SpectrumBaselineY;

        public int DigitalBandPiecesCount;
        public readonly int TotalSpectrumBands = 100;
        public int TotalSpectrumWidth = 10;
        public int SpectrumUseSamples = 8192; //Power Of 2

        public int DrawSpectrumSkipRate = 0;

        private void GetMaxVolume(float[] left, float[] right, ref float lMax, ref float rMax, int start, int length)
        {
            for (int i = 0; i < length && start + i < left.Length; i++)
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

        private void GetAverageVolume(float[] left, float[] right, ref float lAverage, ref float rAverage, int start,
            int length)
        {
            lAverage = 0f;
            rAverage = 0f;
            for (int i = 0; i < length && start + i < left.Length; i++)
            {
                lAverage += left[start + i];

                rAverage += right[start + i];
            }

            lAverage /= length;
            rAverage /= length;
        }

        private void pictureBoxSpectrum_Paint(object sender, PaintEventArgs e)
        {
            if (wmp.playState == WMPPlayState.wmppsPlaying)
            {
                int position = (int) (playerPositionNormalized * currentWavFileData.SamplesCount);

                int pieceLength = currentWavFileData.SampleRate / FramesPerSecond;
                if (position < pieceLength / 2) return;

                int start = (position / pieceLength + 1) * pieceLength;
                int end = Math.Min(currentWavFileData.SamplesCount, (position / pieceLength + 2) * pieceLength);

                int length = end - start;
                //if (length % 2 == 1) length++;

                float maxL = 0;
                float maxR = 0;

                if (start < currentWavFileData.SamplesCount - length)
                {
                    GetMaxVolume(currentWavFileData.LeftChannel, currentWavFileData.RightChannel, ref maxL, ref maxR,
                        start,
                        length);
                }

                CurrentVolumeL += (maxL - CurrentVolumeL) * EasingCoef;
                CurrentVolumeR += (maxR - CurrentVolumeR) * EasingCoef;

                int digitalPartsL = (int) (CurrentVolumeL * DigitalBandPiecesCount);
                int digitalPartsR = (int) (CurrentVolumeR * DigitalBandPiecesCount);

                e.Graphics.DrawLine(Pens.LawnGreen, 0,
                    SpectrumBaselineY - CurrentVolumeL * SpectrumHeight, BandWidth,
                    SpectrumBaselineY - CurrentVolumeL * SpectrumHeight);


                e.Graphics.DrawLine(Pens.OrangeRed, BandWidth,
                    SpectrumBaselineY - CurrentVolumeR * SpectrumHeight,
                    BandWidth + BandWidth,
                    SpectrumBaselineY - CurrentVolumeR * SpectrumHeight);

                for (int i = 1; i < digitalPartsL + 1; i++)
                {
                    e.Graphics.FillRectangle(Brushes.LawnGreen, 0,
                        SpectrumBaselineY - i * (DigitalPieceHeight + DistanceBetweenBands), BandWidth,
                        DigitalPieceHeight);
                }

                for (int i = 1; i < digitalPartsR + 1; i++)
                {
                    e.Graphics.FillRectangle(Brushes.OrangeRed, BandWidth,
                        SpectrumBaselineY - i * (DigitalPieceHeight + DistanceBetweenBands), BandWidth,
                        DigitalPieceHeight);
                }

                if (position < currentWavFileData.SamplesCount - SpectrumUseSamples)
                {
                    float[] newSpectrum =
                        currentWavFileData.GetSpectrumForPosition(
                            playerPositionNormalized,
                            SpectrumUseSamples);
                    for (int i = 0; i < SpectrumUseSamples; i++)
                    {
                        CurrentSpectrum[i] += (newSpectrum[i] - CurrentSpectrum[i]) * EasingCoef;
                    }
                }
                

                //DrawSpectrumBeauty(e.Graphics, CurrentSpectrum);
                DrawSpectrumOriginal(e.Graphics, CurrentSpectrum);
            }
        }

        private void DrawSpectrumOriginal(Graphics g, float[] spectrum)
        {
            int useLength = SpectrumUseSamples / 2;
            int useBands = Math.Min(TotalSpectrumBands, useLength);
            float sbandWidth = (float) TotalSpectrumWidth / useBands;
            float multiplier = 2f; //(float) Math.Log(SpectrumUseSamples, Math.Log(SpectrumUseSamples, 2));

            int lastBand = -1;
            float maxInLastBand = 0f;

            for (int i = 0; i < useLength; i += (1 + DrawSpectrumSkipRate))
            {
                int band = (int) ((float) i / useLength * useBands);
                if (band > lastBand)
                {
                    lastBand = band;
                    maxInLastBand = 0f;
                }

                float normalizedHeight = spectrum[i] * multiplier * (float) Math.Log(i + 2, 2);

                //normalizedHeight *= (float) Math.Log(i + 1, 10); //Применяем логарифмическое выравнивание громкости

                if (normalizedHeight > maxInLastBand)
                {
                    maxInLastBand = normalizedHeight;
                    if (DisplayDigital)
                    {
                        int digitalParts = (int) (normalizedHeight * DigitalBandPiecesCount);
                        
                        for (int k = 1; k < digitalParts + 1; k++)
                        {
                            g.FillRectangle(Brushes.OrangeRed,
                                BandWidth + BandWidth + 50 + band * (sbandWidth + DistanceBetweenBands),
                                SpectrumBaselineY - k * (DigitalPieceHeight + DistanceBetweenBands), sbandWidth,
                                DigitalPieceHeight);
                        }
                    }
                    else
                    {
                        float analogHeight = normalizedHeight * pictureBoxSpectrum.Height;
                        g.FillRectangle(Brushes.OrangeRed,
                            BandWidth + BandWidth + 50 + band * (sbandWidth + DistanceBetweenBands),
                            SpectrumBaselineY - analogHeight,
                            sbandWidth, analogHeight);
                    }
                }
            }
        }

        private void DrawSpectrumBeauty(Graphics g, float[] spectrum)
        {
            int useLength = spectrum.Length / 2;
            int valuesPerBand = (int) ((float) useLength / TotalSpectrumBands);
            float sbandWidth = (float) TotalSpectrumWidth / TotalSpectrumBands;

            for (int i = 0; i < TotalSpectrumBands; i++)
            {
                float maxVal = 0f;
                for (int j = 0; j < valuesPerBand; j++)
                {
                    if (spectrum[i * valuesPerBand + j] > maxVal) maxVal = spectrum[i * valuesPerBand + j];
                    //sumVal += spectrum[i * valuesPerBand + j];
                }

                //sumVal /= valuesPerBand;

                g.FillRectangle(Brushes.LawnGreen, BandWidth + BandWidth + 50 + i * (sbandWidth + DistanceBetweenBands),
                    SpectrumBaselineY - DigitalPieceHeight, sbandWidth,
                    DigitalPieceHeight);

                int digitalParts = (int) (maxVal * DigitalBandPiecesCount);
                for (int k = 1; k < digitalParts + 1; k++)
                {
                    g.FillRectangle(Brushes.OrangeRed,
                        BandWidth + BandWidth + 50 + i * (sbandWidth + DistanceBetweenBands),
                        SpectrumBaselineY - k * (DigitalPieceHeight + DistanceBetweenBands), sbandWidth,
                        DigitalPieceHeight);
                }

                //g.FillRectangle(Brushes.Red,
                //    bandWidth + bandWidth + 50 + sbandWidth * i,
                //    spectrumBaselineY - Math.Abs(spectrumHeight * sumVal),
                //    sbandWidth, Math.Abs(spectrumHeight * sumVal));
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }

        bool pressedOnWaveform;

        private void pictureBoxPlot_MouseDown(object sender, MouseEventArgs e)
        {
            pressedOnWaveform = true;
            wmp.controls.currentPosition = ((float) e.X / pictureBoxPlot.Width) * wmp.currentMedia.duration;
            wmp.controls.play();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            numericUpDown1.Value = (decimal) (EasingCoef * 10);
            numericUpDown2.Value = (decimal) (Math.Log(SpectrumUseSamples, 2));


            OpenFileDialog opf = new OpenFileDialog();
            //opf.Filter = "Файлы WAV (*.wav)|*.wav";
            opf.Filter = "Файлы MP3 (*.mp3)|*.mp3";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                string filename = opf.FileName;

                var reader = new Mp3FileReader(filename);

                MemoryStream ms = new MemoryStream();

                var waveStream = WaveFormatConversionStream.CreatePcmStream(reader);

                WaveFileWriter.WriteWavFileToStream(ms, waveStream);


                ms.Seek(0, SeekOrigin.Begin);

                currentWavFileData = WavFileData.ReadWav(ms);

                SpectrumHeight = pictureBoxSpectrum.Height;
                TotalSpectrumWidth = pictureBoxSpectrum.Width / 2;

                SpectrumBaselineY = pictureBoxSpectrum.Height;

                if (CreateWaveformSequentially)
                {
                    Task.Run(() =>
                    {
                        currentWavFileData.RecreateWaveformBitmapSequentially(
                            pictureBoxPlot.Width, pictureBoxPlot.Height, WaveformSkipSampleRate,
                            ThreadsForWaveformCreation);
                    });
                }
                else
                {
                    Task.Run(() =>
                    {
                        currentWavFileData.RecreateWaveformBitmapParallel(
                            pictureBoxPlot.Width, pictureBoxPlot.Height, WaveformSkipSampleRate,
                            ThreadsForWaveformCreation);
                    });
                }
                
                CurrentSpectrum = currentWavFileData.GetSpectrumForPosition(0, SpectrumUseSamples);

                wmp.currentMedia = wmp.newMedia(filename);
                wmp.controls.play();
                DigitalBandPiecesCount = pictureBoxSpectrum.Height / (DigitalPieceHeight + DistanceBetweenBands);

                timerUpdater.Interval = 1000 / FramesPerSecond;
                timerUpdater.Start();

                this.BringToFront();
            }
            else
            {
                Application.Exit();
            }
        }

        private void pictureBoxPlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (pressedOnWaveform)
            {
                wmp.controls.currentPosition = ((float) e.X / pictureBoxPlot.Width) * wmp.currentMedia.duration;
                //wmp.controls.play();
                pictureBoxPlot.Refresh();
            }
        }

        private void pictureBoxPlot_MouseUp(object sender, MouseEventArgs e)
        {
            pressedOnWaveform = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            EasingCoef = (float) numericUpDown1.Value / 10f;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            SpectrumUseSamples = (int) Math.Pow(2, (double) numericUpDown2.Value);
            CurrentSpectrum = new float[SpectrumUseSamples];
        }
    }
}