using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TapoLib
{
    public class RequestCipher
    {

        readonly private byte[] _Key, _IV;

        public RequestCipher(byte[] Key, byte[] IV)
        {
            _Key = Key;
            _IV = IV;
        }
        
        public string Encrypt(string json)
        {
            byte[] encrypted;

            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(_Key, _IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(json);
                        encrypted = ms.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }
        public string Decrypt(string base64)
        {
            string plaintext = null;
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(_Key, _IV);
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}