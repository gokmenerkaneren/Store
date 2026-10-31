using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Store.IdentityServer.Security
{
    /// <summary>
    /// IdentityServer kullanıcı TCKimlik şifrelemesi için güvenli yardımcı sınıf.
    /// STORE_AES_KEY ortam değişkenini destekler; eski veriler için geriye dönük anahtarlarla çözme güvencesi sunar.
    /// </summary>
    public static class EncryptionHelper
    {
        private static readonly byte[] LegacyKey = Encoding.UTF8.GetBytes("St0reV3ryS3cur3Encrytp1onK3y!!12");
        private static readonly byte[] SharedLegacyKey = SHA256.HashData(Encoding.UTF8.GetBytes("Store_KVKK_SensitiveData_SecretKey_2026_Secure!"));

        public static byte[] GetActiveKey()
        {
            var envKey = Environment.GetEnvironmentVariable("STORE_AES_KEY")
                      ?? Environment.GetEnvironmentVariable("Security__AesKey");

            if (!string.IsNullOrWhiteSpace(envKey))
            {
                return SHA256.HashData(Encoding.UTF8.GetBytes(envKey));
            }

            return LegacyKey;
        }

        public static string Encrypt(string clearText)
        {
            if (string.IsNullOrEmpty(clearText)) return clearText;

            try
            {
                var key = GetActiveKey();
                byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);

                using (Aes encryptor = Aes.Create())
                {
                    encryptor.Key = key;
                    encryptor.GenerateIV();
                    byte[] iv = encryptor.IV;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(iv, 0, iv.Length);

                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch
            {
                return clearText;
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            byte[] cipherBytes;
            try
            {
                cipherBytes = Convert.FromBase64String(cipherText);
            }
            catch
            {
                return cipherText;
            }

            if (cipherBytes.Length < 32) return cipherText;

            // Önce aktif anahtarla dene
            var activeKey = GetActiveKey();
            var decrypted = TryDecrypt(cipherBytes, activeKey);
            if (decrypted != null) return decrypted;

            // Başarısız olursa eski anahtarlarla dene
            if (activeKey != LegacyKey)
            {
                decrypted = TryDecrypt(cipherBytes, LegacyKey);
                if (decrypted != null) return decrypted;
            }

            decrypted = TryDecrypt(cipherBytes, SharedLegacyKey);
            if (decrypted != null) return decrypted;

            return cipherText;
        }

        private static string? TryDecrypt(byte[] cipherBytes, byte[] key)
        {
            try
            {
                using (Aes decryptor = Aes.Create())
                {
                    decryptor.Key = key;

                    byte[] iv = new byte[16];
                    Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                    decryptor.IV = iv;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, iv.Length, cipherBytes.Length - iv.Length);
                            cs.Close();
                        }
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
