using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Store.WebUI.Controllers
{
    [Route("Favorite")]
    public class FavoriteController : Controller
    {
        private readonly IFavoriteApiService _favoriteApiService;
        private readonly ICatalogApiService _catalogApiService;
        private readonly ILogger<FavoriteController> _logger;

        public FavoriteController(
            IFavoriteApiService favoriteApiService,
            ICatalogApiService catalogApiService,
            ILogger<FavoriteController> logger)
        {
            _favoriteApiService = favoriteApiService;
            _catalogApiService = catalogApiService;
            _logger = logger;
        }

        [HttpPost("Toggle")]
        public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ProductId))
            {
                return BadRequest(new { success = false, message = "Geçersiz ürün bilgisi." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "Favorilere eklemek için giriş yapmalısınız." });
            }

            var token = await HttpContext.GetTokenAsync("access_token");
            var authHeader = $"Bearer {token}";

            try
            {
                if (request.IsFavorite)
                {
                    await _favoriteApiService.AddFavoriteAsync(new AddFavoriteDto { UserId = userId, ProductId = request.ProductId }, authHeader);
                }
                else
                {
                    await _favoriteApiService.RemoveFavoriteAsync(userId, request.ProductId, authHeader);
                }

                return Ok(new { success = true, productId = request.ProductId, isFavorite = request.IsFavorite });
            }
            catch (Refit.ApiException ex)
            {
                _logger.LogWarning(ex, "Favori işlemi Refit API hatası: {StatusCode} {Message}", ex.StatusCode, ex.Message);
                return StatusCode((int)ex.StatusCode, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Favori işlemi genel hata: {Message}", ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Remove")]
        public async Task<IActionResult> RemoveFavorite([FromBody] RemoveFavoriteRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ProductId))
            {
                return BadRequest(new { success = false, message = "Geçersiz ürün bilgisi." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "Favorileri yönetmek için giriş yapmalısınız." });
            }

            var token = await HttpContext.GetTokenAsync("access_token");
            var authHeader = $"Bearer {token}";

            try
            {
                await _favoriteApiService.RemoveFavoriteAsync(userId, request.ProductId, authHeader);
                return Ok(new { success = true, productId = request.ProductId });
            }
            catch (Refit.ApiException ex)
            {
                _logger.LogWarning(ex, "Favori silme Refit API hatası: {StatusCode}", ex.StatusCode);
                return StatusCode((int)ex.StatusCode, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Favori silme genel hata: {Message}", ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("MyFavorites")]
        public async Task<IActionResult> MyFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Ok(new List<string>()); // Giriş yapılmamışsa boş liste döndür
            }

            var token = await HttpContext.GetTokenAsync("access_token");
            var authHeader = $"Bearer {token}";

            try
            {
                var favorites = await _favoriteApiService.GetUserFavoritesAsync(userId, authHeader);
                return Ok(favorites ?? new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MyFavorites alınırken hata: {Message}", ex.Message);
                return Ok(new List<string>());
            }
        }

        [HttpGet("GetFavoritesDetail")]
        public async Task<IActionResult> GetFavoritesDetail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Ok(new { success = true, count = 0, items = new List<object>() });
            }

            var token = await HttpContext.GetTokenAsync("access_token");
            var authHeader = $"Bearer {token}";

            List<string>? favoriteIds = null;
            try
            {
                favoriteIds = await _favoriteApiService.GetUserFavoritesAsync(userId, authHeader);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetFavoritesDetail: Favori ID listesi alınamadı: {UserId}", userId);
            }

            if (favoriteIds == null || !favoriteIds.Any())
            {
                return Ok(new { success = true, count = 0, items = new List<object>() });
            }

            var cultureTr = new CultureInfo("tr-TR");
            var itemTasks = favoriteIds.Select(async id =>
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    return null;
                }

                try
                {
                    var product = await _catalogApiService.GetProductByIdAsync(guidId);
                    if (product == null) return null;

                    string imageUrl = "/images/no-image.jpg";
                    try
                    {
                        var images = await _catalogApiService.GetProductImagesByProductIdAsync(guidId);
                        var mainImg = images?.FirstOrDefault(x => x.IsMain)?.ImageUrl ?? images?.FirstOrDefault()?.ImageUrl;
                        if (!string.IsNullOrWhiteSpace(mainImg))
                        {
                            imageUrl = mainImg;
                        }
                    }
                    catch (Exception imgEx)
                    {
                        _logger.LogDebug(imgEx, "Favori ürün görseli alınamadı: {ProductId}", id);
                    }

                    decimal price = 0;
                    try
                    {
                        var prices = await _catalogApiService.GetProductPricesByProductIdAsync(guidId);
                        if (prices != null && prices.Any())
                        {
                            var activePrice = prices.FirstOrDefault(p => p.IsActive && !p.IsDeleted) ?? prices.FirstOrDefault();
                            if (activePrice != null)
                            {
                                price = activePrice.DiscountedAmount ?? activePrice.Amount;
                            }
                        }
                    }
                    catch (Exception priceEx)
                    {
                        _logger.LogDebug(priceEx, "Favori ürün fiyatı alınamadı: {ProductId}", id);
                    }

                    if (price == 0 && product.Price > 0)
                    {
                        price = product.Price;
                    }

                    return new
                    {
                        productId = id,
                        productName = product.Name ?? "Ürün",
                        imageUrl = imageUrl,
                        price = price,
                        formattedPrice = price.ToString("N2", cultureTr) + " ₺",
                        productUrl = $"/Product/Index/{id}"
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Favori ürün detayı katalogdan alınamadı: {ProductId}", id);
                    return null;
                }
            });

            var results = await Task.WhenAll(itemTasks);
            var validItems = results.Where(x => x != null).ToList();

            return Ok(new
            {
                success = true,
                count = validItems.Count,
                items = validItems
            });
        }
    }

    public class FavoriteRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
    }

    public class RemoveFavoriteRequest
    {
        public string ProductId { get; set; } = string.Empty;
    }
}
