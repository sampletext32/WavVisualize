using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WavVisualize
{
    //Класс отвечающий за работу с Wav файлом
    public class WavFileData
    {
        #region Параметры Wav файла

        public int ChunkId;
        public int FileSize;
        public int RiffType;
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
        public int BytesLength;
        public int BytesForSample;
        public int SamplesCount;

        #endregion

        #region Используемые для вычислений данные

        public float[] LeftChannel;
        public float[] RightChannel;

        #endregion

        //функция рассчитывает спектр для заданного количества сэмплов начиная с нормализованной позиции
        public float[] GetSpectrumForPosition(float position, FFTProvider fftProvider)
        {
            int start = (int) (SamplesCount * position);
            fftProvider.Calculate(LeftChannel, start);
            float[] spectrum = fftProvider.Get();
            return spectrum;
        }

        //функция читает Wav файл из потока
        public static WavFileData ReadWav(Stream stream)
        {
            WavFileData wavFileData = new WavFileData();
            try
            {
                using (stream)
                {
                    BinaryReader reader = new BinaryReader(stream);

                    wavFileData.ChunkId = reader.ReadInt32();
                    wavFileData.FileSize = reader.ReadInt32();
                    wavFileData.RiffType = reader.ReadInt32();

                    wavFileData.FmtId = reader.ReadInt32();
                    wavFileData.FmtSize = reader.ReadInt32();
                    wavFileData.FmtCode = reader.ReadInt16();
                    wavFileData.Channels = reader.ReadInt16();
                    wavFileData.SampleRate = reader.ReadInt32();
                    wavFileData.ByteRate = reader.ReadInt32();
                    wavFileData.FmtBlockAlign = reader.ReadInt16();
                    wavFileData.BitDepth = reader.ReadInt16();

                    if (wavFileData.FmtSize == 18)
                    {
                        wavFileData.FmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(wavFileData.FmtExtraSize);
                    }

                    wavFileData.DataId = reader.ReadInt32();
                    wavFileData.BytesLength = reader.ReadInt32();

                    byte[] data = reader.ReadBytes(wavFileData.BytesLength);

                    wavFileData.BytesForSample =
                        wavFileData.BitDepth / 8; //байт на сэмпл - (глубина кодирования в битах) / 8(бит в байте)

                    //количество сэмплов =  общее_количество_байт   / байт_на_сэмпл/ количество_каналов
                    //                      Сумма                   / Скорость      / Параллельность
                    wavFileData.SamplesCount =
                        wavFileData.BytesLength / wavFileData.BytesForSample / wavFileData.Channels;

                    float[] asFloat;
                    switch (wavFileData.BitDepth)
                    {
                        case 64:
                            //количество значений = количество_сэмплов * количество_каналов
                            var asDouble = new double[wavFileData.SamplesCount * wavFileData.Channels];
                            Buffer.BlockCopy(data, 0, asDouble, 0, wavFileData.BytesLength);
                            asFloat = Array.ConvertAll(asDouble, e => (float) e);
                            break;
                        case 32:
                            //количество значений = количество_сэмплов * количество_каналов
                            asFloat = new float[wavFileData.SamplesCount * wavFileData.Channels];
                            Buffer.BlockCopy(data, 0, asFloat, 0, wavFileData.BytesLength);
                            break;
                        case 16:
                            //количество значений = количество_сэмплов * количество_каналов
                            var asInt16 = new short[wavFileData.SamplesCount * wavFileData.Channels];
                            Buffer.BlockCopy(data, 0, asInt16, 0, wavFileData.BytesLength);
                            asFloat = Array.ConvertAll(asInt16,
                                e => e / (float) short.MaxValue); //нормализуем, деля на максимальное значение
                            break;
                        default: throw new FormatException("Unknown BitDepth");
                    }

                    switch (wavFileData.Channels)
                    {
                        case 1:
                            //если записано МОНО, оба канала = исходному
                            wavFileData.LeftChannel = asFloat;
                            wavFileData.RightChannel = asFloat;
                            break;
                        case 2:
                            //если записано СТЕРЕО
                            //создаём 2 массива на левый и правый канал с количеством сэмплов
                            wavFileData.LeftChannel = new float[wavFileData.SamplesCount];
                            wavFileData.RightChannel = new float[wavFileData.SamplesCount];
                            for (int i = 0, s = 0; i < wavFileData.SamplesCount; i++)
                            {
                                //записываем сэмплы
                                wavFileData.LeftChannel[i] = asFloat[s++];
                                wavFileData.RightChannel[i] = asFloat[s++];
                            }

                            break;
                        default: throw new FormatException("Unknown Channels");
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return wavFileData;
        }

        //функция читает Wav файл из массива байт
        public static WavFileData ReadWav(byte[] stream)
        {
            WavFileData wavFileData = ReadWav(new MemoryStream(stream));

            return wavFileData;
        }

        //функция читает Wav файл, открывая файл по переданному пути
        public static WavFileData ReadWav(string filename)
        {
            WavFileData wavFileData = ReadWav(File.OpenRead(filename));

            return wavFileData;
        }
    }
}