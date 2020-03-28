using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WavVisualize
{
    class AES128Decryptor
    {
        public static byte[] Decrypt(byte[] data, byte[] key, byte[] ivBytes)
        {
            AesManaged aes = new AesManaged();
            aes.Mode = CipherMode.CBC;
            aes.Key = key;
            aes.IV = ivBytes;
            var decryptor = aes.CreateDecryptor();
            byte[] result = new byte[data.Length];
            decryptor.TransformBlock(data, 0, data.Length, result, 0);
            return result;
        }
    }
}
