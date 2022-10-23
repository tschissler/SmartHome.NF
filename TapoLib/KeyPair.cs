using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TapoLib
{
    public class KeyPair
    {
        private byte[] _privateKey;
        private byte[] _publicKey;

        public KeyPair()
        {
            var rsa = RSA.Create(1024);

            /*_privateKey = rsa.ExportPkcs8PrivateKey("top secret",
                new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 1)
                );*/

            _privateKey = rsa.ExportPkcs8PrivateKey();

            _publicKey = rsa.ExportSubjectPublicKeyInfo();
        }

        public string GetPublicKeyPem()
        {
            /*return new string(PemEncoding.Write("PUBLIC KEY", _publicKey));*/
            return @"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC7TXQ4gygC+9RgtrYdnaIglI5W
U649kudfB1cof8uRFALCcviOUlZtKY3YgxoL8Ep4B231nrS2uAdGgu3g5uKGvLD9
KdrIcUqGkchg1taJvSNcgkZnuaXKEAFKYQTz/imKpjSvHQJBViMmusOehuEBuJ2D
SsNFX0YEurNEZW5MFwIDAQAB
-----END PUBLIC KEY-----";
        }

        public string Encrypt(string data)
        {
            var rsa = RSA.Create(1024);
            /*rsa.ImportSubjectPublicKeyInfo(_publicKey, out int bRead);*/
            rsa.ImportFromPem(@"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC7TXQ4gygC+9RgtrYdnaIglI5W
U649kudfB1cof8uRFALCcviOUlZtKY3YgxoL8Ep4B231nrS2uAdGgu3g5uKGvLD9
KdrIcUqGkchg1taJvSNcgkZnuaXKEAFKYQTz/imKpjSvHQJBViMmusOehuEBuJ2D
SsNFX0YEurNEZW5MFwIDAQAB
-----END PUBLIC KEY-----");

            byte[] decryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(data), RSAEncryptionPadding.Pkcs1);

            return Convert.ToBase64String(decryptedBytes);
        }

        public byte[] Decrypt(string base64)
        {
            var rsa = RSA.Create(1024);
            /*rsa.ImportEncryptedPkcs8PrivateKey("top secret", _privateKey, out int bytesRead);*/
            /*rsa.ImportPkcs8PrivateKey(_privateKey, out int bytesRead);*/

            rsa.ImportFromEncryptedPem(@"-----BEGIN ENCRYPTED PRIVATE KEY-----
MIIC3TBXBgkqhkiG9w0BBQ0wSjApBgkqhkiG9w0BBQwwHAQIvtspvyd14XQCAggA
MAwGCCqGSIb3DQIJBQAwHQYJYIZIAWUDBAEqBBAoD7tnCCwLPqHdbN1naChKBIIC
gC/qpl2Pj8KEbqTK6g3iaKjO0BhL2w5BnKSD6cAvIkvGkFrBioRQlq+AhbJu+e8c
UO7MdeSY/C5HatJZVVA5aNSL2MpqWDjUDSu+ZkrF5CF51nxN6BIH9Qshq07FYtO0
HmhZTDpqQCQoomwst6hfqsXKZoTnx4afV5B5XAmDkbRu2yjMsArYVWic6Wl/FaJc
gFlt6iDhfp8am/VZLr5cGyx2JYzs9B/R79XjVqNWdYDr7p6s4uu9jb9+5owm/tpB
pA3GsCeZLjdsbZM9ltXwLn+gJuDDkbMj29kOszdIiBM7QCkFZsSVGEKtpSt0HeuL
DWTBNPpvIhyWCEWvxwErKNXgSrCnYiQ7jVb0HUbTwj36J+tKnoXbutkl81lqcGSf
jtiYTgMcxcZ8z9l0PdrOVFdLlp05VnP2E5fgoEDUuXgAHllTHZNr6CGQzH6zVleu
7+kSX1uanFlVKdG7zJ2sZ9+4DG65i3z2DGKJguKOwpXlgoCXFgZ8F8b9ZFPUnN9d
a3SYph7HC/FdLF8/+KslwU9+DxnqQHDuEOMOsXssJzTA0OcWCopHc3m1NaqwbrW+
8tIFZlT8hTSQZ9AlU9q38hP4apt7PMazuVxZAoa1ox9EgW/mY3zGsSLX4FOfo8jZ
Udmc+QLg303QPB/SASJIqGQsj4KKWOe9bhr3ZmwyQp7WvqvWwWz8NUXX4U2p8OdJ
b15394z3YS+HDRzxOwz0NdVn9keYZnmVpbJQRjZIhWS+HG+2oWVemyqjpN9U7NQ3
kFB2Nrx2ycOBd9AZFUG3vbIlBChtOkDlDt2jf+7RdxIuqNXDMtWaJAgwDhm9oQB/
CklQqigfv1CbIFydlOOvgsA=
-----END ENCRYPTED PRIVATE KEY-----", "top secret");


            byte[] decryptedBytes = rsa.Decrypt(Convert.FromBase64String(base64), RSAEncryptionPadding.Pkcs1);

            Console.WriteLine(Convert.ToBase64String(decryptedBytes));

            return decryptedBytes;
        }
    }
}
