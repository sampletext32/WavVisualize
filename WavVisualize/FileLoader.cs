using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

namespace WavVisualize
{
    public class FileLoader
    {
        public static event Action OnBeginMp3Decompression;
        public static event Action OnBeginWavWriting;

        public static async Task<byte[]> LoadAndDecompressMp3(byte[] fileBytes)
        {
            using (MemoryStream mp3MemoryStream = new MemoryStream(fileBytes))
            {
                //открываем файл
                var reader = new Mp3FileReader(mp3MemoryStream);

                OnBeginMp3Decompression?.Invoke();

                //создаём PCM поток
                var waveStream = WaveFormatConversionStream.CreatePcmStream(reader);

                MemoryStream ms = new MemoryStream();

                OnBeginWavWriting?.Invoke();

                //переписываем MP3 в Wav файл в потоке
                await Task.Run(() => WaveFileWriter.WriteWavFileToStream(ms, waveStream));

                return ms.ToArray();
            }
        }

        public static async Task<byte[]> LoadAny(string filepath)
        {
            byte[] fileBytes = File.ReadAllBytes(filepath);
            if (filepath.EndsWith(".mp3"))
            {
                return await LoadAndDecompressMp3(fileBytes);
            }
            else if (filepath.EndsWith(".wav"))
            {
                return fileBytes;
            }
            else
            {
                throw new NotSupportedException("FileFormat Unknown");
            }
        }
    }
}