using Store.Order.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Store.Order.Presentation.WebApi.Services
{
    /// <summary>
    /// JWT token içindeki client_id claim'ini okuyarak Guid döner.
    /// client_id bir Guid formatında değilse MD5 ile deterministik Guid'e dönüştürülür.
    /// </summary>
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetClientId()
        {
            var clientId = _httpContextAccessor.HttpContext?.User?
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
