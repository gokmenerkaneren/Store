using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Store.Catalog.Services.CacheServices
{
    public class CatalogCacheService : ICatalogCacheService
    {
        /// <summary>
        /// IDistributedCache'in her anahtarın başına eklediği önek. Program.cs'teki kayıtla aynı değer olmalıdır.
        /// </summary>
        public const string InstanceName = "CatalogCache_";

        /// <summary>
        /// Süre verilmezse kullanılır; böylece hiçbir kayıt Redis'te süresiz kalmaz.
        /// </summary>
        public static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(1);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly ILogger<CatalogCacheService> _logger;

        public CatalogCacheService(
            IDistributedCache cache,
            IConnectionMultiplexer multiplexer,
            ILogger<CatalogCacheService> logger)
        {
            _cache = cache;
            _multiplexer = multiplexer;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var (_, value) = await TryGetAsync<T>(key);
            return value;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (value is null)
            {
                return;
            }

            try
            {
                var payload = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
                };

                await _cache.SetAsync(key, payload, options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache write failed for key {CacheKey}.", key);
            }
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var (found, cached) = await TryGetAsync<T>(key);
            if (found)
            {
                return cached;
            }

            // Bilinçli olarak try dışında: veri kaynağı hataları önbellek hatası gibi yutulmamalı.
            var value = await factory();
            await SetAsync(key, value, expiration);
            return value;
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache remove failed for key {CacheKey}.", key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            EndPoint[] endpoints;
            IDatabase database;

            try
            {
                endpoints = _multiplexer.GetEndPoints();
                database = _multiplexer.GetDatabase();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache invalidation failed for prefix {CachePrefix}.", prefix);
                return;
            }

            foreach (var endpoint in endpoints)
            {
                // Bir sunucudaki hata diğer sunuculardaki temizliği engellememeli.
                try
                {
                    await RemoveFromServerAsync(_multiplexer.GetServer(endpoint), database, prefix);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Cache invalidation failed on {RedisEndpoint} for prefix {CachePrefix}.", endpoint, prefix);
                }
            }
        }

        private async Task RemoveFromServerAsync(IServer server, IDatabase database, string prefix)
        {
            if (!server.IsConnected)
            {
                _logger.LogWarning("Skipping cache invalidation on disconnected Redis server {RedisEndpoint} for prefix {CachePrefix}.", server.EndPoint, prefix);
                return;
            }

            // Replikalar birincil sunucunun anahtarlarını tekrar listeler.
            if (server.IsReplica)
            {
                return;
            }

            // Redis'teki gerçek anahtarlar InstanceName ile başlar. Listelenen anahtar zaten bu öneki
            // taşıdığından IDatabase ile silinir: IDistributedCache.RemoveAsync öneki ikinci kez ekler.
            await foreach (var key in server.KeysAsync(database: database.Database, pattern: $"{InstanceName}{prefix}*"))
            {
                await database.KeyDeleteAsync(key);
            }
        }

        private async Task<(bool Found, T? Value)> TryGetAsync<T>(string key)
        {
            try
            {
                var payload = await _cache.GetAsync(key);
                if (payload is null)
                {
                    return (false, default);
                }

                var value = JsonSerializer.Deserialize<T>(payload, JsonOptions);
                return (value is not null, value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache read failed for key {CacheKey}.", key);
                return (false, default);
            }
        }
    }
}
