using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Store.Shared.Security
{
    /// <summary>
    /// KVKK uyumluluğu için hassas kişisel verileri (TCKN, Vergi No vb.) AES-256 ile şifreleyen ve çözen yardımcı sınıf.
    /// Her şifreleme işleminde benzersiz bir IV (Initialization Vector) üretilir ve şifreli metnin başına eklenir.
    /// </summary>
    public static class AesEncryptionHelper
    {
        // 256-bit (32 byte) sabit anahtar (İleride Azure Key Vault / ortam değişkeninden de beslenebilir)
        private static readonly byte[] Key = SHA256.HashData(Encoding.UTF8.GetBytes("Store_KVKK_SensitiveData_SecretKey_2026_Secure!"));

        public static string? Encrypt(string? plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                return plainText;
            }

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            
            // İlk 16 byte IV olarak yazılır
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string? Decrypt(string? cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
            {
                return cipherText;
            }

            byte[] fullCipher;
            try
            {
                fullCipher = Convert.FromBase64String(cipherText);
            }
            catch
            {
                // Base64 değilse (örn. eski düz metin veri), doğrudan metni geri döndür
                return cipherText;
            }

            // IV (16 byte) + en az 1 blok veri (16 byte) = en az 32 byte olmalı
            if (fullCipher.Length < 32)
            {
                return cipherText;
            }

            try
            {
                using var aes = Aes.Create();
                aes.Key = Key;

                var iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, 16);
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(fullCipher, 16, fullCipher.Length - 16);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs, Encoding.UTF8);

                return sr.ReadToEnd();
            }
            catch
            {
                // Şifre çözülemezse (örneğin eski düz metin veri Base64 formatına benziyorsa), ham veriyi koru
                return cipherText;
            }
        }
    }
}
