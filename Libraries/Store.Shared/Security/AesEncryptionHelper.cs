using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Store.Shared.Security
{
    /// <summary>
    /// KVKK uyumluluğu için hassas kişisel verileri (TCKN, Vergi No vb.) AES-256 ile şifreleyen ve çözen yardımcı sınıf.
    /// Çoklu anahtar geçişi (Key Rotation), ortam değişkeni/User Secrets desteği,
    /// eski veritabanı kayıtlarını bozmayan geri uyumlu şifre çözme ve AES-GCM kimlik doğrulamalı şifreleme desteği içerir.
    /// </summary>
    public static class AesEncryptionHelper
    {
        // Geriye dönük uyumluluk için eski sabit anahtarlar
        private static readonly byte[] LegacyKey1 = SHA256.HashData(Encoding.UTF8.GetBytes("Store_KVKK_SensitiveData_SecretKey_2026_Secure!"));
        private static readonly byte[] LegacyKey2 = Encoding.UTF8.GetBytes("St0reV3ryS3cur3Encrytp1onK3y!!12");

        private static byte[]? _configuredKey;

        /// <summary>
        /// Test veya uygulama başlangıcında özel bir anahtar tanımlamak için kullanılır.
        /// </summary>
        public static void SetActiveKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _configuredKey = null;
                return;
            }
            _configuredKey = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        }

        /// <summary>
        /// Aktif şifreleme anahtarını ortam değişkeninden (STORE_AES_KEY / Security__AesKey) veya konfigürasyondan alır.
        /// Tanımlanmamışsa geriye dönük güvenli anahtara fallback yapar.
        /// </summary>
        public static byte[] GetActiveKey()
        {
            if (_configuredKey != null) return _configuredKey;

            var envKey = Environment.GetEnvironmentVariable("STORE_AES_KEY") 
                      ?? Environment.GetEnvironmentVariable("Security__AesKey");

            if (!string.IsNullOrWhiteSpace(envKey))
            {
                return SHA256.HashData(Encoding.UTF8.GetBytes(envKey));
            }

            return LegacyKey1;
        }

        /// <summary>
        /// Standart AES-CBC (256-bit) ile rastgele IV kullanarak şifreler.
        /// </summary>
        public static string? Encrypt(string? plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                return plainText;
            }

            var key = GetActiveKey();
            using var aes = Aes.Create();
            aes.Key = key;
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

        /// <summary>
        /// AES-GCM (Authenticated Encryption) standardında şifreler.
        /// Çıktı formatı: gcm:base64(12_byte_nonce + 16_byte_tag + ciphertext)
        /// </summary>
        public static string? EncryptGcm(string? plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText)) return plainText;

            var key = GetActiveKey();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] nonce = new byte[12]; // 96-bit nonce
            RandomNumberGenerator.Fill(nonce);

            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[16]; // 128-bit authentication tag

            using var aesGcm = new AesGcm(key, 16);
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

            byte[] combined = new byte[nonce.Length + tag.Length + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherBytes, 0, combined, nonce.Length + tag.Length, cipherBytes.Length);

            return "gcm:" + Convert.ToBase64String(combined);
        }

        /// <summary>
        /// Şifreli metni çözer. Önce aktif anahtarla, başarısız olursa eski (legacy) anahtarlarla dener.
        /// Hem AES-CBC hem de modern AES-GCM formatlarını otomatik tanır.
        /// </summary>
        public static string? Decrypt(string? cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
            {
                return cipherText;
            }

            // 1. AES-GCM formatı kontrolü
            if (cipherText.StartsWith("gcm:", StringComparison.OrdinalIgnoreCase))
            {
                var decryptedGcm = TryDecryptGcm(cipherText.Substring(4), GetActiveKey())
                                ?? TryDecryptGcm(cipherText.Substring(4), LegacyKey1)
                                ?? TryDecryptGcm(cipherText.Substring(4), LegacyKey2);

                return decryptedGcm ?? cipherText;
            }

            byte[] fullCipher;
            try
            {
                fullCipher = Convert.FromBase64String(cipherText);
            }
            catch
            {
                return cipherText;
            }

            if (fullCipher.Length < 32)
            {
                return cipherText;
            }

            // 2. Çoklu anahtarla deneme (Key Rotation & Migration Güvencesi)
            var activeKey = GetActiveKey();
            var decrypted = TryDecryptCbc(fullCipher, activeKey);
            if (decrypted != null) return decrypted;

            if (activeKey != LegacyKey1)
            {
                decrypted = TryDecryptCbc(fullCipher, LegacyKey1);
                if (decrypted != null) return decrypted;
            }

            decrypted = TryDecryptCbc(fullCipher, LegacyKey2);
            if (decrypted != null) return decrypted;

            return cipherText;
        }

        private static string? TryDecryptCbc(byte[] fullCipher, byte[] key)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = key;

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
                return null;
            }
        }

        private static string? TryDecryptGcm(string base64Data, byte[] key)
        {
            try
            {
                byte[] combined = Convert.FromBase64String(base64Data);
                if (combined.Length < 28) return null; // 12 nonce + 16 tag minimum

                byte[] nonce = new byte[12];
                byte[] tag = new byte[16];
                int cipherLength = combined.Length - 28;
                byte[] cipherBytes = new byte[cipherLength];
                byte[] plainBytes = new byte[cipherLength];

                Buffer.BlockCopy(combined, 0, nonce, 0, 12);
                Buffer.BlockCopy(combined, 12, tag, 0, 16);
                Buffer.BlockCopy(combined, 28, cipherBytes, 0, cipherLength);

                using var aesGcm = new AesGcm(key, 16);
                aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

                return Encoding.UTF8.GetString(plainBytes);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Eski anahtarla şifrelenmiş veriyi çözüp, aktif geçerli anahtarla yeniden şifreler (Veri Göçü / Re-encryption).
        /// </summary>
        public static string? ReEncrypt(string? cipherText, bool upgradeToGcm = false)
        {
            if (string.IsNullOrWhiteSpace(cipherText)) return cipherText;

            var plainText = Decrypt(cipherText);
            if (plainText == cipherText) return cipherText; // Çözülemediyse değiştirme

            return upgradeToGcm ? EncryptGcm(plainText) : Encrypt(plainText);
        }
    }
}
