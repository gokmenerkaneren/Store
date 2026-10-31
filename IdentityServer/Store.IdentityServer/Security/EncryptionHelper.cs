using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Store.IdentityServer.Security
{
    public static class EncryptionHelper
    {
        // KVKK Encryption Key (Must be 32 bytes for AES-256). In production, keep this in Environment Variables!
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("St0reV3ryS3cur3Encrytp1onK3y!!12"); 

        public static string Encrypt(string clearText)
        {
            if (string.IsNullOrEmpty(clearText)) return clearText;

            try
            {
                byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    encryptor.Key = Key;
                    // Generate a random IV for each encryption
                    encryptor.GenerateIV();
                    byte[] iv = encryptor.IV;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Write IV to the beginning of the stream
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

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    encryptor.Key = Key;
                    
                    // Extract the IV from the first 16 bytes
                    byte[] iv = new byte[16];
                    Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                    encryptor.IV = iv;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
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
                // If decryption fails (e.g., it was plaintext before this feature), return original
                return cipherText;
            }
        }
    }
}
