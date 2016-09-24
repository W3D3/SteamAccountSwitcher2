using System;
using System.IO;
using System.Security.Cryptography;

// ReSharper disable once CheckNamespace
namespace NuGet.Modules
{
    public static class AesHelper
    {
        private const int IvLength = 16;
        private const int SaltLength = 8;
        private const int KeyLength = 32;
        private const int IterationCount = 10000;

        public static byte[] Encrypt(byte[] data, string password)
        {
            var iv = CreateBytes(IvLength);
            var salt = CreateBytes(SaltLength);
            var key = new Rfc2898DeriveBytes(password, salt, IterationCount).GetBytes(KeyLength);
            using (var algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm?.CreateEncryptor(key, iv))
            {
                var encryptData = Crypt(data, encryptor);
                var result = new byte[iv.Length + salt.Length + encryptData.Length];
                iv.CopyTo(result, 0);
                salt.CopyTo(result, iv.Length);
                encryptData.CopyTo(result, iv.Length + salt.Length);
                return result;
            }
        }

        public static byte[] Decrypt(byte[] data, string password)
        {
            var iv = new byte[IvLength];
            var salt = new byte[SaltLength];
            var encryptData = new byte[data.Length - salt.Length - iv.Length];
            Array.Copy(data, 0, iv, 0, iv.Length);
            Array.Copy(data, iv.Length, salt, 0, salt.Length);
            Array.Copy(data, iv.Length + salt.Length, encryptData, 0, encryptData.Length);
            var key = new Rfc2898DeriveBytes(password, salt, IterationCount).GetBytes(KeyLength);
            using (var algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm?.CreateDecryptor(key, iv))
            {
                return Crypt(encryptData, decryptor);
            }
        }

        private static byte[] Crypt(byte[] data, ICryptoTransform cryptor)
        {
            var m = new MemoryStream();
            using (Stream c = new CryptoStream(m, cryptor, CryptoStreamMode.Write))
            {
                c.Write(data, 0, data.Length);
            }
            return m.ToArray();
        }

        private static byte[] CreateBytes(int length)
        {
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                var data = new byte[length];
                rngCsp.GetBytes(data);
                return data;
            }
        }
    }
}