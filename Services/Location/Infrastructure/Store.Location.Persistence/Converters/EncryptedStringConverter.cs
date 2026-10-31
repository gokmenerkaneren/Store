using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Store.Shared.Security;

namespace Store.Location.Persistence.Converters
{
    /// <summary>
    /// EF Core ValueConverter: C# entity tarafındaki düz metin TCKN vb. verileri
    /// PostgreSQL veritabanına yazarken AES-256 ile şifreler, veritabanından okurken çözer.
    /// </summary>
    public class EncryptedStringConverter : ValueConverter<string?, string?>
    {
        public EncryptedStringConverter()
            : base(
                v => AesEncryptionHelper.Encrypt(v),
                v => AesEncryptionHelper.Decrypt(v))
        {
        }
    }
}
