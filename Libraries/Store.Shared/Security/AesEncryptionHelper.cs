using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Store.Shared.Security
{
    /// <summary>
    /// AES-256 Şifreleme ve Anahtar Rotasyonu / Veri Güvenliği Yardımcısı.
    /// K3 Direktifi:
    /// 1. Sırları koddan çıkarıp Ortam Değişkeni / User Secrets üzerinden dinamik okur.
    /// 2. Eski verileri bozmamak adına kademeli geri dönüş (fallback) şifre çözme mekanizması sunar.
    /// 3. Eski şifreli verileri yeni aktif anahtara veya AES-GCM'e taşımak için ReEncrypt API'si sunar.
    /// 4. Kimlik doğrulamalı şifreleme (Authenticated Encryption - AES-GCM) desteği sağlar.
    /// </summary>
    public static class AesEncryptionHelper
    {
        // Geriye dönük uyumluluk için eski bilinen statik anahtarlar (Fallback Keys)
        private static readonly byte[] LegacyKey1 = Encoding.UTF8.GetBytes("A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6"); // 32 byte
        private static readonly byte[] LegacyKey2 = Encoding.UTF8.GetBytes("K1L2M3N4O5P6Q7R8S9T0U1V2W3X4Y5Z6"); // 32 byte

        // Çalışma zamanında (runtime) veya testlerde değiştirilebilir aktif anahtar
        private static byte[]? _customActiveKey;

        /// <summary>
        /// Test veya dinamik rotasyon senaryolarında aktif anahtarı ayarlamak için kullanılır.
        /// </summary>
        public static void SetActiveKey(string? base64OrUtf8Key)
        {
            if (string.IsNullOrWhiteSpace(base64OrUtf8Key))
            {
                _customActiveKey = null;
                return;
            }

            _customActiveKey = DeriveKey32(base64OrUtf8Key);
        }

        /// <summary>
        /// Öncelik sırasına göre aktif geçerli anahtarı belirler:
        /// 1. Runtime SetActiveKey() ile belirlenen anahtar
        /// 2. STORE_AES_KEY veya Security__AesKey ortam değişkeni
        /// 3. Varsayılan LegacyKey1
        /// </summary>
        public static byte[] GetActiveKey()
        {
            if (_customActiveKey != null && _customActiveKey.Length == 32)
            {
                return _customActiveKey;
            }

            var envKey = Environment.GetEnvironmentVariable("STORE_AES_KEY")
                      ?? Environment.GetEnvironmentVariable("Security__AesKey");

            if (!string.IsNullOrWhiteSpace(envKey))
            {
                return DeriveKey32(envKey);
            }

            return LegacyKey1;
        }

        private static byte[] DeriveKey32(string key)
        {
            try
            {
                var base64Bytes = Convert.FromBase64String(key);
                if (base64Bytes.Length == 32) return base64Bytes;
            }
            catch
            {
                // Base64 değilse UTF-8 SHA256 ile 32 byte'a türet
            }

            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        /// <summary>
        /// Standart AES-CBC (256-bit) ile şifreler (Geriye dönük uyumlu format).
        /// </summary>
        public static string? Encrypt(string? plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                return plainText;
            }

            using var aes = Aes.Create();
            aes.Key = GetActiveKey();
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// AES-GCM (Authenticated Encryption) ile şifreler.
        /// Çıktı: "gcm:" + Base64(nonce [12B] + tag [16B] + ciphertext [NB])
        /// </summary>
        public static string? EncryptGcm(string? plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                return plainText;
            }

            byte[] key = GetActiveKey();
            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[16];

            using var aesGcm = new AesGcm(key, 16);
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

            byte[] combined = new byte[12 + 16 + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, combined, 0, 12);
            Buffer.BlockCopy(tag, 0, combined, 12, 16);
            Buffer.BlockCopy(cipherBytes, 0, combined, 28, cipherBytes.Length);

            return "gcm:" + Convert.ToBase64String(combined);
        }

        /// <summary>
        /// Veriyi çözer. Çoklu anahtar (Active Key -> LegacyKey1 -> LegacyKey2) kademeli fallback uygular.
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

            if (!activeKey.SequenceEqual(LegacyKey1))
            {
                decrypted = TryDecryptCbc(fullCipher, LegacyKey1);
                if (decrypted != null) return decrypted;
            }

            if (!activeKey.SequenceEqual(LegacyKey2))
            {
                decrypted = TryDecryptCbc(fullCipher, LegacyKey2);
                if (decrypted != null) return decrypted;
            }

            return cipherText;
        }

        private static string? TryDecryptCbc(byte[] fullCipher, byte[] key)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = key;
                aes.Padding = PaddingMode.PKCS7;

                var iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, 16);
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(fullCipher, 16, fullCipher.Length - 16);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                
                // Strict UTF-8: Hatalı anahtarla çözülen rastgele bayt dizilerinde DecoderFallbackException fırlatır
                var strictUtf8 = new UTF8Encoding(false, true);
                using var sr = new StreamReader(cs, strictUtf8);

                var result = sr.ReadToEnd();
                // Yanlış anahtarla açılan veride kontrol karakterleri (çöp veri) oluşursa geçersiz say
                if (result.Any(c => char.IsControl(c) && c != '\r' && c != '\n' && c != '\t'))
                {
                    return null;
                }

                return result;
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

                var strictUtf8 = new UTF8Encoding(false, true);
                return strictUtf8.GetString(plainBytes);
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
