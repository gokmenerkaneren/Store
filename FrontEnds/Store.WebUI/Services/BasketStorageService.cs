using Store.DtoLayer.BasketDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Store.WebUI.Services
{
    public interface IBasketStorageService
    {
        Task<BasketTotalDto> GetBasketAsync();
        Task SaveBasketAsync(BasketTotalDto basket);
        Task DeleteBasketAsync();
    }

    public class BasketStorageService : IBasketStorageService
    {
        private readonly IBasketApiService _basketApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BasketStorageService> _logger;

        // Mikroservis veya Redis çevrimdışı iken sepetin kaybolmasını engelleyen thread-safe yerel bellek deposu
        private static readonly ConcurrentDictionary<string, BasketTotalDto> _localBaskets = new();

        public BasketStorageService(
            IBasketApiService basketApiService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<BasketStorageService> logger)
        {
            _basketApiService = basketApiService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private string GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) 
                         ?? user?.FindFirstValue("sub") 
                         ?? user?.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userId))
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    if (httpContext.Request.Cookies.TryGetValue("guest_basket_id", out var guestId) && !string.IsNullOrEmpty(guestId))
                    {
                        return "guest_" + guestId;
                    }
                    var newGuestId = Guid.NewGuid().ToString("N");
                    try
                    {
                        httpContext.Response.Cookies.Append("guest_basket_id", newGuestId, new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30),
                            HttpOnly = true,
                            IsEssential = true
                        });
                    }
                    catch { }
                    return "guest_" + newGuestId;
                }
                userId = "default-user";
            }
            return userId.ToLowerInvariant();
        }

        public async Task<BasketTotalDto> GetBasketAsync()
        {
            var userId = GetUserId();

            try
            {
                var fetchTask = _basketApiService.GetMyBasketDetailAsync();
                var completedTask = await Task.WhenAny(fetchTask, Task.Delay(1000));
                if (completedTask == fetchTask)
                {
                    var remoteBasket = await fetchTask;
                    if (remoteBasket != null)
                    {
                        remoteBasket.BasketItems ??= new();
                        _localBaskets[userId] = remoteBasket;
                        return remoteBasket;
                    }
                }
                else
                {
                    _logger.LogWarning("Store.Basket mikroservisi 1000ms içinde yanıt vermedi. Yerel depodan dönülüyor.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Store.Basket mikroservisine ulaşılamadı ({Msg}). Yerel sepet deposundan getiriliyor.", ex.Message);
            }

            if (_localBaskets.TryGetValue(userId, out var localBasket) && localBasket != null)
            {
                localBasket.BasketItems ??= new();
                return localBasket;
            }

            var emptyBasket = new BasketTotalDto { BasketItems = new() };
            _localBaskets[userId] = emptyBasket;
            return emptyBasket;
        }

        public async Task SaveBasketAsync(BasketTotalDto basket)
        {
            var userId = GetUserId();
            basket.BasketItems ??= new();
            _localBaskets[userId] = basket;

            // Uzak servise kaydetme işlemini arka planda dene, yanıt gelmezse kullanıcıyı bekletme
            _ = Task.Run(async () =>
            {
                try
                {
                    var saveTask = _basketApiService.SaveMyBasketAsync(basket);
                    var delayTask = Task.Delay(1500);
                    if (await Task.WhenAny(saveTask, delayTask) == saveTask)
                    {
                        await saveTask;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Store.Basket mikroservisine kaydedilemedi ({Msg}). Yerel depoda saklandı.", ex.Message);
                }
            });

            await Task.CompletedTask;
        }

        public async Task DeleteBasketAsync()
        {
            var userId = GetUserId();
            _localBaskets.TryRemove(userId, out _);

            _ = Task.Run(async () =>
            {
                try
                {
                    var deleteTask = _basketApiService.DeleteMyBasketAsync();
                    var delayTask = Task.Delay(1500);
                    if (await Task.WhenAny(deleteTask, delayTask) == deleteTask)
                    {
                        await deleteTask;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Store.Basket mikroservisinden sepet silinemedi: {Msg}", ex.Message);
                }
            });

            await Task.CompletedTask;
        }
    }
}
