using Store.Basket.Dtos;
using Store.Basket.Settings;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Store.Basket.Services
{
    public class BasketService : IBasketService
    {
        private readonly RedisService _redisService;
        private readonly ILogger<BasketService> _logger;
        // Redis sunucusu çevrimdışı iken sepet akışının kesilmemesini sağlayan in-memory resilience deposu
        private static readonly ConcurrentDictionary<string, string> _memoryStore = new();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        public BasketService(RedisService redisService, ILogger<BasketService> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        public async Task DeleteBasket(Guid userId)
        {
            var key = userId.ToString();
            _memoryStore.TryRemove(key, out _);

            try
            {
                var db = _redisService.GetDb();
                if (db != null)
                {
                    await db.KeyDeleteAsync(key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis üzerinden sepet silinirken hata oluştu, bellek deposu temizlendi.");
            }
        }

        public async Task<BasketTotalDto> GetBasket(Guid userId)
        {
            var key = userId.ToString();
            string? basketJson = null;

            try
            {
                var db = _redisService.GetDb();
                if (db != null)
                {
                    basketJson = await db.StringGetAsync(key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis üzerinden sepet okunurken hata oluştu, in-memory depoya geçiliyor.");
            }

            if (string.IsNullOrEmpty(basketJson))
            {
                _memoryStore.TryGetValue(key, out basketJson);
            }

            if (string.IsNullOrEmpty(basketJson))
            {
                return new BasketTotalDto
                {
                    UserId = userId,
                    BasketItems = new System.Collections.Generic.List<BasketItemDto>()
                };
            }

            try
            {
                return JsonSerializer.Deserialize<BasketTotalDto>(basketJson!, _jsonOptions)
                    ?? new BasketTotalDto { UserId = userId, BasketItems = new() };
            }
            catch
            {
                return new BasketTotalDto
                {
                    UserId = userId,
                    BasketItems = new System.Collections.Generic.List<BasketItemDto>()
                };
            }
        }

        public async Task SaveBasket(BasketTotalDto basketTotalDto)
        {
            var key = basketTotalDto.UserId.ToString();
            var basketJson = JsonSerializer.Serialize(basketTotalDto, _jsonOptions);

            // 1. Önce in-memory depoya yaz (güvenli yedekleme)
            _memoryStore[key] = basketJson;

            // 2. Redis varsa Redis'e senkronize et
            try
            {
                var db = _redisService.GetDb();
                if (db != null)
                {
                    await db.StringSetAsync(key, basketJson, TimeSpan.FromDays(30));
                }
                else
                {
                    _logger.LogInformation("Redis bağlantısı aktif değil, sepet bellek deposuna (in-memory) kaydedildi.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis'e sepet kaydedilirken hata oluştu, in-memory yedekleme aktif.");
            }
        }
    }
}
