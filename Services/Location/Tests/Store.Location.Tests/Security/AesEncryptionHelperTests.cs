using FluentAssertions;
using Store.Location.Persistence.Converters;
using Store.Shared.Security;
using Xunit;

namespace Store.Location.Tests.Security
{
    public class AesEncryptionHelperTests
    {
        [Theory]
        [InlineData("12345678901")]
        [InlineData("98765432100")]
        [InlineData("TCKN_TEST_DATA")]
        public void Encrypt_And_Decrypt_Should_Preserve_Original_Value(string plainText)
        {
            // Act
            string cipherText = AesEncryptionHelper.Encrypt(plainText);
            string decrypted = AesEncryptionHelper.Decrypt(cipherText);

            // Assert
            cipherText.Should().NotBeNullOrEmpty();
            cipherText.Should().NotBe(plainText);
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void Encrypt_Should_Produce_Different_Ciphertext_For_Same_Input_Due_To_Random_IV()
        {
            // Arrange
            string tckn = "11122233344";

            // Act
            string cipher1 = AesEncryptionHelper.Encrypt(tckn);
            string cipher2 = AesEncryptionHelper.Encrypt(tckn);

            // Assert
            cipher1.Should().NotBe(cipher2);
            AesEncryptionHelper.Decrypt(cipher1).Should().Be(tckn);
            AesEncryptionHelper.Decrypt(cipher2).Should().Be(tckn);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Encrypt_And_Decrypt_Should_Handle_Null_Or_Empty(string? input)
        {
            // Act & Assert
            AesEncryptionHelper.Encrypt(input!).Should().Be(input);
            AesEncryptionHelper.Decrypt(input!).Should().Be(input);
        }

        [Fact]
        public void EncryptedStringConverter_Should_Correctly_Convert_To_And_From_Provider()
        {
            // Arrange
            var converter = new EncryptedStringConverter();
            var toProvider = converter.ConvertToProviderExpression.Compile();
            var fromProvider = converter.ConvertFromProviderExpression.Compile();
            string plainText = "12345678901";

            // Act
            string encrypted = toProvider(plainText);
            string decrypted = fromProvider(encrypted);

            // Assert
            encrypted.Should().NotBe(plainText);
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_Should_Fallback_To_Legacy_Key_When_ActiveKey_Rotated()
        {
            // Arrange: Eski anahtarla şifrelenmiş veri simülasyonu
            AesEncryptionHelper.SetActiveKey(null!); // Default legacy key
            string plainText = "22334455667";
            string legacyCipher = AesEncryptionHelper.Encrypt(plainText)!;

            // Act: Yeni anahtara geçiş (Key Rotation)
            AesEncryptionHelper.SetActiveKey("New_Secure_Super_Key_2026_Environment_Secret!");
            string decrypted = AesEncryptionHelper.Decrypt(legacyCipher)!;

            // Assert: Eski veri yeni anahtar aktifken de başarıyla çözülebilmeli
            decrypted.Should().Be(plainText);

            // Cleanup
            AesEncryptionHelper.SetActiveKey(null!);
        }

        [Fact]
        public void EncryptGcm_And_Decrypt_Should_Preserve_Original_Value_AuthenticatedEncryption()
        {
            // Arrange
            string plainText = "TCKN_GCM_SECURE_99887766554";

            // Act
            string? gcmCipher = AesEncryptionHelper.EncryptGcm(plainText);
            string? decrypted = AesEncryptionHelper.Decrypt(gcmCipher);

            // Assert
            gcmCipher.Should().NotBeNull();
            gcmCipher.Should().StartWith("gcm:");
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void ReEncrypt_Should_Successfully_Migrate_Data_To_Active_Key()
        {
            // Arrange
            AesEncryptionHelper.SetActiveKey(null!);
            string plainText = "55443322110";
            string legacyCipher = AesEncryptionHelper.Encrypt(plainText)!;

            // Act: Yeni anahtar ile re-encrypt
            AesEncryptionHelper.SetActiveKey("Rotated_Active_Key_2026!");
            string reEncrypted = AesEncryptionHelper.ReEncrypt(legacyCipher, upgradeToGcm: true)!;
            string decrypted = AesEncryptionHelper.Decrypt(reEncrypted)!;

            // Assert
            reEncrypted.Should().StartWith("gcm:");
            decrypted.Should().Be(plainText);

            // Cleanup
            AesEncryptionHelper.SetActiveKey(null!);
        }
    }
}
