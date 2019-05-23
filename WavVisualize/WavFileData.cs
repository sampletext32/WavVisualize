using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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

        public Bitmap WaveformBitmap;

        public void RecreateWaveformBitmap(int width, int height)
        {
            WaveformBitmap = new Bitmap(width, height);
            float yScale = 0.8f;
            using (Graphics g = Graphics.FromImage(WaveformBitmap))
            {
                float yCenter = height / 2f;
                if (LeftChannel != null && RightChannel != null)
                {
                    for (int i = 0; i < LeftChannel.Length; i++)
                    {
                        float xPosition = (float) i / LeftChannel.Length * width;
                        float valueL = LeftChannel[i] * (height / 2f) * yScale;
                        float valueR = RightChannel[i] * (height / 2f) * yScale;

                        g.FillRectangle(Brushes.LawnGreen, xPosition, yCenter - valueL, 1, valueL);

                        g.FillRectangle(Brushes.Red, xPosition, yCenter, 1, valueR);
                    }
                }
            }
        }


        /// <summary>
        /// Provides the Discrete Fourier Transform for a real-valued input signal
        /// </summary>
        /// <param name="inputReal">the signal to transform</param>
        /// <param name="partials">the maximum number of partials to calculate. If no value is given it defaults to input.length / 2</param>
        /// <returns>The real and imaginary part of each partial</returns>
        public static float[] DFT(float[] inputReal, int start, int end)
        {
            int len = end - start;
            float fdtCoef = (float) (2 * Math.PI / len);
            float[] cosDFT = new float[len];
            //float[] sinDFT = outputImag;

            int[] ints = new int[len];
            for (var i = 0; i < ints.Length; i++)
            {
                ints[i] = i;
            }

            ints.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount * 8).ForAll(v =>
            {
                float cos = 0.0f;
                //float sin = 0.0f;

                for (int i = 0; i < len; i++)
                {
                    cos += (float) (inputReal[start + i] * Math.Cos(fdtCoef * v * i));
                    //sin -= (float)(inputReal[i] * Math.Sin(2 * Math.PI * n / len * i));
                }

                cosDFT[v] = cos;
            });

            //for (int n = 0; n < len; n++)
            //{
            //    float cos = 0.0f;
            //    //float sin = 0.0f;
            //
            //    for (int i = 0; i < len; i++)
            //    {
            //        cos += (float) (inputReal[start + i] * Math.Cos(fdtCoef * n * i));
            //        //sin -= (float)(inputReal[i] * Math.Sin(2 * Math.PI * n / len * i));
            //    }
            //
            //    cosDFT[n] = cos;
            //    //sinDFT[n] = sin;
            //}

            return cosDFT;
        }

        public static WavFileData ReadWav(string filename)
        {
            WavFileData wavFileData = new WavFileData();
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

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
            catch
            {
                return null;
            }

            return wavFileData;
        }
    }
}