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
    }
}
