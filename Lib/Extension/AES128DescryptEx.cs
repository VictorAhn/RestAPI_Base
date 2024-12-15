using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Extension
{
    public enum AES128Type
    {
        Encrypt = 0,
        Decrypt = 1
    }
    public static class AES128DescryptEx
    {
        public static byte[] Key = ASCIIEncoding.ASCII.GetBytes("TestKeys");
        /// <summary>
        /// AES128 암호화 (키 지정)
        /// </summary>
        /// <param name="type">암호화/복호화</param>
        /// <param name="input">입력 문자열</param>
        /// <param name="key">암호화 키</param>
        /// <returns></returns>
        public static string Result(AES128Type type, string input, string key = "TestKeys")
        {
            byte[] keyBytes = ASCIIEncoding.ASCII.GetBytes(key);

            using (var des = new DESCryptoServiceProvider()
            {
                Key = keyBytes,
                IV = keyBytes,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            using (var ms = new MemoryStream())
            {
                ICryptoTransform transform = type.Equals(AES128Type.Encrypt) ? des.CreateEncryptor() : des.CreateDecryptor();

                byte[] data = type.Equals(AES128Type.Encrypt) ? Encoding.UTF8.GetBytes(input) : Convert.FromBase64String(input);

                using (var cryStream = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                {
                    cryStream.Write(data, 0, data.Length);
                    cryStream.FlushFinalBlock();
                }

                byte[] resultBytes = ms.ToArray();

                return type.Equals(AES128Type.Encrypt) ? Convert.ToBase64String(resultBytes) : Encoding.UTF8.GetString(resultBytes).RemoveSpace();
            }
        }
    }
}
