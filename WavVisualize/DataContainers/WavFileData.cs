using System;
using System.IO;

namespace WavVisualize
{
    //Класс отвечающий за работу с Wav файлом
    public class WavFileData
    {
        #region Параметры Wav файла

        //Done, using https://audiocoding.ru/articles/2008-05-22-wav-file-structure/

        //TODO: ENCAPSULATION FIX !IMPORTANT

        /*0  -  3*/ /* 4 */
        public String chunkId; //содержит символы "RIFF"

        /*4  -  7*/ /* 4 */
        public int chunkSize; //размер оставшейся цепочки - размер файла минус 8 байт

        /*8  - 11*/ /* 4 */
        public String format; //содержит символы "WAVE"

        /*12 - 15*/ /* 4 */
        public String subchunk1Id; //содержит символы "fmt "

        /*16 - 19*/ /* 4 */
        public int subchunk1Size; //16 для PCM - размер оставшейся подцепочки

        /*20 - 21*/ /* 2 */
        public short
            audioFormat; //1 для PCM TODO: extend https://audiocoding.ru/articles/2008-05-22-wav-file-structure/wav_formats.txt

        /*22 - 23*/ /* 2 */
        public short numChannels; //количество каналов

        /*24 - 27*/ /* 4 */
        public int sampleRate; //частота дискретизации

        /*28 - 31*/ /* 4 */
        public int byteRate; //количество байт в секунду воспроизведения

        /*32 - 33*/ /* 2 */
        public short blockAlign; //байт на 1 сэмпл включая все каналы

        /*34 - 35*/ /* 2 */
        public short bitsPerSample; //количество бит в сэмпле или глубина кодирования

        /*36 - 39*/ /* 4 */
        public String subchunk2Id; //содержит символы "data"

        /*40 - 43*/ /* 4 */
        public int subchunk2Size; //количество байт в области данных

        /*44 -inf*/ /* 0 */
        byte[] data; //сами WAV данные

        #endregion

        #region Используемые для вычислений данные

        public float[] RawSamples;

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

        private float[] ExtractSamples(byte[] data)
        {
            int length = data.Length;
            float[] samples;
            //TODO: Increase performance by excluding every step POW calculation
            switch (bitsPerSample)
            {
                case 8:
                    samples = new float[length];
                    for (int i = 0; i < samples.Length; i++)
                    {
                        int sample = (int) (data[i * 1 + 0] & 0xff);
                        samples[i] = (float) (sample / Math.Pow(2, 8));
                    }

                    break;
                case 16:
                    samples = new float[length / 2];
                    for (int i = 0; i < samples.Length; i++)
                    {
                        short sample = (short) (
                            ((data[i * 2 + 0] & 0xff) << 0) | ((data[i * 2 + 1] & 0xff) << 8));
                        samples[i] = (float) (sample / Math.Pow(2, 16));
                    }

                    break;
                case 24:
                    samples = new float[length / 3];
                    for (int i = 0; i < samples.Length; i++)
                    {
                        //т.к. 24 бит влезает только в 32 бит число, записываем как есть и смещаем всё на 8 бит влево.
                        int sample
                            = (((data[i * 3 + 0] & 0xff) << 0) | ((data[i * 3 + 1] & 0xff) << 8) |
                               ((data[i * 3 + 2] & 0xff) << 16)) << 8;
                        samples[i] =
                            (float) (sample / Math.Pow(2,
                                         32)); //Делим на 2^32 - максимальное число в 32 битном числе, а не 24.
                    }

                    break;
                case 32:
                    samples = new float[length / 4];
                    for (int i = 0; i < samples.Length; i++)
                    {
                        int sample
                            = ((data[i * 4 + 0] & 0xff) << 0) | ((data[i * 4 + 1] & 0xff) << 8) |
                              ((data[i * 4 + 2] & 0xff) << 16) | ((data[i * 4 + 3] & 0xff) << 24);
                        samples[i] = (float) (sample / Math.Pow(2, 32));
                    }

                    break;
                case 64:
                    samples = new float[length / 8];
                    for (int i = 0; i < samples.Length; i++)
                    {
                        long sample =
                            ((long) (data[i * 8 + 0] & 0xff) << 0) | ((long) (data[i * 8 + 1] & 0xff) << 8) |
                            ((long) (data[i * 8 + 2] & 0xff) << 16) | ((long) (data[i * 8 + 3] & 0xff) << 24) |
                            ((long) (data[i * 8 + 4] & 0xff) << 32) | ((long) (data[i * 8 + 5] & 0xff) << 40) |
                            ((long) (data[i * 8 + 6] & 0xff) << 48) | ((long) (data[i * 8 + 7] & 0xff) << 56);
                        samples[i] = (float) (sample / Math.Pow(2, 64));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown BitDepth");
            }

            return samples;
        }

        public WavFileData(byte[] fileData)
        {
            using (var ms = new MemoryStream(fileData))
            {
                BinaryReader reader = new BinaryReader(ms);

                chunkId = "" + (char) reader.ReadByte() + (char) reader.ReadByte() + (char) reader.ReadByte() +
                          (char) reader.ReadByte(); // 0x52494646
                chunkSize = reader.ReadInt32();
                format = "" + (char) reader.ReadByte() + (char) reader.ReadByte() + (char) reader.ReadByte() +
                         (char) reader.ReadByte(); // 0x57415645
                subchunk1Id = "" + (char) reader.ReadByte() + (char) reader.ReadByte() + (char) reader.ReadByte() +
                              (char) reader.ReadByte(); // 0x666d7420
                subchunk1Size = reader.ReadInt32();
                audioFormat = reader.ReadInt16();
                numChannels = reader.ReadInt16();
                sampleRate = reader.ReadInt32();
                byteRate = reader.ReadInt32();
                blockAlign = reader.ReadInt16();
                bitsPerSample = reader.ReadInt16();
                subchunk2Id = "" + (char) reader.ReadByte() + (char) reader.ReadByte() + (char) reader.ReadByte() +
                              (char) reader.ReadByte(); // 0x64617461
                subchunk2Size = reader.ReadInt32();
                data = reader.ReadBytes((int) (ms.Length - ms.Position));

                RawSamples = ExtractSamples(data);

                switch (numChannels)
                {
                    case 1:
                        //если записано МОНО, оба канала = исходному
                        LeftChannel = RawSamples;
                        RightChannel = RawSamples;
                        break;
                    case 2:
                        //если записано СТЕРЕО
                        //создаём 2 массива на левый и правый канал с количеством сэмплов
                        LeftChannel = new float[RawSamples.Length / numChannels];
                        RightChannel = new float[RawSamples.Length / numChannels];
                        for (int i = 0, s = 0; i < RawSamples.Length / numChannels; i++)
                        {
                            //записываем сэмплы
                            LeftChannel[i] = RawSamples[s++];
                            RightChannel[i] = RawSamples[s++];
                        }

                        break;
                    default: throw new FormatException("Unknown Channels");
                }
            }
        }
    }
}