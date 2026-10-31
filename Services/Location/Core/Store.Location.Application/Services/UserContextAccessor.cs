namespace Store.Location.Application.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Static helper to obtain the client_id claim from the JWT and convert it to Guid.
    /// Used in Location handlers to replace hard‑coded audit GUIDs.
    /// </summary>
    public static class UserContextAccessor
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Initialize(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static Guid GetClientId()
        {
            var clientId = _httpContextAccessor?.HttpContext?.User?.FindFirst("client_id")?.Value;
            if (string.IsNullOrEmpty(clientId))
                return Guid.Empty;

            if (Guid.TryParse(clientId, out var guid))
                return guid;

            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(clientId));
            return new Guid(hash);
        }
    }
}
