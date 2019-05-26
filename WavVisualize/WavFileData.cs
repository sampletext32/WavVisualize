using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WavVisualize
{
    class WavFileData
    {
        public int ChunkId;
        public int FileSize;

        public int RiffType;

        // chunk 1
        public int FmtId;
        public int FmtSize;
        public int FmtExtraSize;
        public int FmtCode;
        public int Channels;
        public int SampleRate;
        public int ByteRate;
        public int FmtBlockAlign;
        public int BitDepth;

        public int DataId;
        public int Bytes;

        public byte[] ByteArray;

        public int BytesForSample;
        public int SamplesCount;

        public float[] LeftChannel;
        public float[] RightChannel;

        public Bitmap[] WaveformBitmaps;

        public void RecreateWaveformBitmapSequentially(int width, int height, int skip = 0, int threads = 1)
        {
            Color greenColor = Color.FromArgb(255 / 10, Color.LawnGreen);
            Brush[] greenBrushes = new Brush[threads];

            Color redColor = Color.FromArgb(255 / 10, Color.OrangeRed);
            Brush[] redBrushes = new Brush[threads];

            float yScale = 0.8f;
            float yCenter = height / 2f;
            WaveformBitmaps = new Bitmap[threads];
            for (int b = 0; b < threads; b++)
            {
                WaveformBitmaps[b] = new Bitmap(width / threads, height);

                greenBrushes[b] = new SolidBrush(greenColor);
                redBrushes[b] = new SolidBrush(redColor);

                int t = b;
                Task.Run(() =>
                {
                    using (Graphics g = Graphics.FromImage(WaveformBitmaps[t]))
                    {
                        if (LeftChannel != null && RightChannel != null)
                        {
                            for (int i = 0; i < SamplesCount / threads; i += (1 + skip))
                            {
                                float xPosition = (float) i / SamplesCount * width;
                                float valueL = LeftChannel[t * SamplesCount / threads + i] * (height / 2f) * yScale;
                                float valueR = RightChannel[t * SamplesCount / threads + i] * (height / 2f) * yScale;

                                g.FillRectangle(greenBrushes[t], xPosition, yCenter - valueL, 1, valueL);

                                g.FillRectangle(redBrushes[t], xPosition, yCenter, 1, valueR);
                            }
                        }
                    }
                }).ContinueWith((task) =>
                {
                    greenBrushes[t].Dispose();
                    redBrushes[t].Dispose();
                });
            }
        }

        public void RecreateWaveformBitmapParallel(int width, int height, int skip = 0, int threads = 1)
        {
            Color greenColor = Color.FromArgb(255 / 10, Color.LawnGreen);
            Brush[] greenBrushes = new Brush[threads];

            Color redColor = Color.FromArgb(255 / 10, Color.OrangeRed);
            Brush[] redBrushes = new Brush[threads];

            float yScale = 0.8f;
            float yCenter = height / 2f;
            WaveformBitmaps = new Bitmap[threads];
            for (int b = 0; b < threads; b++)
            {
                WaveformBitmaps[b] = new Bitmap(width, height);
                greenBrushes[b] = new SolidBrush(greenColor);
                redBrushes[b] = new SolidBrush(redColor);
                int t = b;
                Task.Run(() =>
                {
                    using (Graphics g = Graphics.FromImage(WaveformBitmaps[t]))
                    {
                        if (LeftChannel != null && RightChannel != null)
                        {
                            for (int i = t; i < SamplesCount; i += threads * (1 + skip))
                            {
                                float xPosition = (float) i / SamplesCount * width;
                                float valueL = LeftChannel[i] * (height / 2f) * yScale;
                                float valueR = RightChannel[i] * (height / 2f) *
                                               yScale;

                                g.FillRectangle(greenBrushes[t], xPosition, yCenter - valueL, 1, valueL);

                                g.FillRectangle(redBrushes[t], xPosition, yCenter, 1, valueR);
                            }
                        }
                    }
                }).ContinueWith((task) =>
                {
                    greenBrushes[t].Dispose();
                    redBrushes[t].Dispose();
                });
            }
        }

        public float GetVolumeL(float position)
        {
            return LeftChannel[(int) (SamplesCount * position)];
        }

        public float GetVolumeR(float position)
        {
            return LeftChannel[(int) (SamplesCount * position)];
        }

        public float[] GetSpectrumForPosition(float position, int spectrumUseSamples)
        {
            float[] spectrum = MyFFT.FFT(LeftChannel, (int) (SamplesCount * position), spectrumUseSamples);
            return spectrum;
        }

        public static WavFileData ReadWav(Stream stream)
        {
            WavFileData wavFileData = new WavFileData();
            try
            {
                using (stream)
                {
                    BinaryReader reader = new BinaryReader(stream);

                    // chunk 0
                    wavFileData.ChunkId = reader.ReadInt32();
                    wavFileData.FileSize = reader.ReadInt32();
                    wavFileData.RiffType = reader.ReadInt32();


                    // chunk 1
                    wavFileData.FmtId = reader.ReadInt32();
                    wavFileData.FmtSize = reader.ReadInt32(); // bytes for this chunk
                    wavFileData.FmtCode = reader.ReadInt16();
                    wavFileData.Channels = reader.ReadInt16();
                    wavFileData.SampleRate = reader.ReadInt32();
                    wavFileData.ByteRate = reader.ReadInt32();
                    wavFileData.FmtBlockAlign = reader.ReadInt16();
                    wavFileData.BitDepth = reader.ReadInt16();

                    if (wavFileData.FmtSize == 18)
                    {
                        // Read any extra values
                        wavFileData.FmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(wavFileData.FmtExtraSize);
                    }

                    // chunk 2
                    wavFileData.DataId = reader.ReadInt32();
                    wavFileData.Bytes = reader.ReadInt32();

                    // DATA!
                    wavFileData.ByteArray = reader.ReadBytes(wavFileData.Bytes);

                    wavFileData.BytesForSample = wavFileData.BitDepth / 8;
                    wavFileData.SamplesCount = wavFileData.Bytes / wavFileData.BytesForSample / wavFileData.Channels;


                    float[] asFloat;
                    switch (wavFileData.BitDepth)
                    {
                        case 64:
                            var asDouble = new double[wavFileData.SamplesCount * wavFileData.Channels];
                            Buffer.BlockCopy(wavFileData.ByteArray, 0, asDouble, 0, wavFileData.Bytes);
                            asFloat = Array.ConvertAll(asDouble, e => (float) e);
                            break;
                        case 32:
                            asFloat = new float[wavFileData.SamplesCount * wavFileData.Channels];
                            Buffer.BlockCopy(wavFileData.ByteArray, 0, asFloat, 0, wavFileData.Bytes);
                            break;
                        case 16:
                            var asInt16 = new short[wavFileData.SamplesCount * wavFileData.Channels];
                            Buffer.BlockCopy(wavFileData.ByteArray, 0, asInt16, 0, wavFileData.Bytes);
                            asFloat = Array.ConvertAll(asInt16, e => e / (float) short.MaxValue);
                            break;
                        default: throw new FormatException("Unknown BitDepth");
                    }

                    switch (wavFileData.Channels)
                    {
                        case 1:
                            wavFileData.LeftChannel = asFloat;
                            wavFileData.RightChannel = asFloat;
                            break;
                        case 2:
                            wavFileData.LeftChannel = new float[wavFileData.SamplesCount];
                            wavFileData.RightChannel = new float[wavFileData.SamplesCount];
                            for (int i = 0, s = 0; i < wavFileData.SamplesCount; i++)
                            {
                                wavFileData.LeftChannel[i] = asFloat[s++];
                                wavFileData.RightChannel[i] = asFloat[s++];
                            }

                            break;
                        default: throw new FormatException("Unknown Channels");
                    }
                }
            }
            catch(Exception ex)
            {
                return null;
            }

            return wavFileData;
        }

        public static WavFileData ReadWav(byte[] stream)
        {
            WavFileData wavFileData = ReadWav(new MemoryStream(stream));

            return wavFileData;
        }

        public static WavFileData ReadWav(string filename)
        {
            WavFileData wavFileData = ReadWav(File.OpenRead(filename));

            return wavFileData;
        }
    }
}