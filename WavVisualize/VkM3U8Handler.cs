using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    public class VkM3U8Handler
    {
        public static byte[] LoadFile(string m3u8)
        {
            MemoryStream memoryStream = new MemoryStream();
            string[] lines = m3u8.Split('\n');
            bool hasKey = false;
            string currentKeyUrl = "";
            byte[] currentKeyBytes = null;
            string dataUrl = "";
            int IVint = -1;
            byte[] IVbytes = new byte[16];
            int extifsCount = 0;
            int written = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#"))
                {
                    //Handle META
                    if (lines[i].StartsWith("#EXT-X-KEY"))
                    {
                        //Save current KEY
                        //#EXT-X-KEY:METHOD=AES-128,URI="*"
                        if (lines[i].StartsWith("#EXT-X-KEY:METHOD=AES-128"))
                        {
                            currentKeyUrl = lines[i].Substring(31, lines[i].Length - 31 - 1);
                            currentKeyBytes = WebAccessor.LoadBytes(currentKeyUrl);
                            dataUrl = currentKeyUrl.Substring(0, currentKeyUrl.IndexOf("key.pub"));
                            hasKey = true;
                        }
                        else if (lines[i].StartsWith("#EXT-X-KEY:METHOD=NONE"))
                        {
                            currentKeyUrl = "";
                            hasKey = false;
                        }
                    }
                    else if (lines[i].StartsWith("#EXT-X-MEDIA-SEQUENCE:"))
                    {
                        IVint = Convert.ToInt32(lines[i].Remove(0, 22));
                        var ivBytes = BitConverter.GetBytes(IVint);
                        IVbytes[12] = ivBytes[3];
                        IVbytes[13] = ivBytes[2];
                        IVbytes[14] = ivBytes[1];
                        IVbytes[15] = ivBytes[0];
                    }
                    else if (lines[i].StartsWith("#EXTINF") && extifsCount != 0)
                    {
                        IVint++;
                        var ivBytes = BitConverter.GetBytes(IVint);
                        IVbytes[12] = ivBytes[3];
                        IVbytes[13] = ivBytes[2];
                        IVbytes[14] = ivBytes[1];
                        IVbytes[15] = ivBytes[0];
                    }
                    else if (lines[i].StartsWith("#EXTINF") && extifsCount == 0)
                    {
                        extifsCount++;
                    }
                }
                else
                {
                    //This is a media URL

                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string m3u8pieceUrl = dataUrl + lines[i];

                    var m3u8pieceBytes = WebAccessor.LoadBytes(m3u8pieceUrl);

                    if (hasKey)
                    {
                        //Decode
                        byte[] decoded = AES128Decryptor.Decrypt(m3u8pieceBytes, currentKeyBytes, IVbytes);

                        if (written < 10)
                        {
                            File.WriteAllBytes(
                                written.ToString() + "_" + lines[i].Substring(0, lines[i].IndexOf(".ts")) + ".ts", decoded);
                            written++;
                        }
                        else
                        {
                            //return null;
                        }

                        //FileLoader.LoadAndDecompressMp3(decoded).GetAwaiter().GetResult();

                        memoryStream.Write(decoded, 0, decoded.Length);
                    }
                    else
                    {
                        if (written < 10)
                        {
                            File.WriteAllBytes(written.ToString() + "_" + lines[i].Substring(0, lines[i].IndexOf(".ts")) + ".ts", m3u8pieceBytes);
                            written++;
                        }
                        else
                        {
                            //return null;
                        }

                        //FileLoader.LoadAndDecompressMp3(m3u8pieceBytes).GetAwaiter().GetResult();
                        memoryStream.Write(m3u8pieceBytes, 0, m3u8pieceBytes.Length);
                    }
                }
            }

            return memoryStream.ToArray();
        }
    }
}