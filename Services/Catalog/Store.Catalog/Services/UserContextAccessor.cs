using System.Security.Cryptography;
using System.Text;

namespace Store.Catalog.Services
{
    /// <summary>
    /// Catalog servisi için JWT token'dan client_id claim'ini okuyarak Guid döndüren statik yardımcı sınıf.
    /// Program.cs başlangıcında Initialize() ile IHttpContextAccessor enjekte edilmelidir.
    /// </summary>
    public static class UserContextAccessor
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        /// <summary>
        /// Program.cs'de çağrılarak IHttpContextAccessor'ı başlatır.
        /// </summary>
        public static void Initialize(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// JWT token içindeki client_id claim'ini okuyarak Guid döner.
        /// client_id geçerli bir Guid değilse MD5 ile deterministik Guid'e dönüştürülür.
        /// </summary>
        public static Guid GetClientId()
        {
            var clientId = _httpContextAccessor?.HttpContext?.User?
                .FindFirst("client_id")?.Value;

            if (string.IsNullOrEmpty(clientId))
                return Guid.Empty;

            // Eğer zaten geçerli bir Guid ise direkt kullan
            if (Guid.TryParse(clientId, out var guid))
                return guid;

            // Değilse (örn. "StoreAdminId") MD5 ile deterministik Guid üret
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(clientId));
            return new Guid(hash);
        }
    }
}

