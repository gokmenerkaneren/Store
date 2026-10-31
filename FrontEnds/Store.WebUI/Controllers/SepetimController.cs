using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.DtoLayer.BasketDtos;
using Store.WebUI.Services;
using Store.WebUI.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.WebUI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class SepetimController : Controller
    {
        private readonly IBasketStorageService _basketStorageService;
        private readonly ICatalogApiService _catalogApiService;
        private readonly ILogger<SepetimController> _logger;

        public SepetimController(
            IBasketStorageService basketStorageService,
            ICatalogApiService catalogApiService,
            ILogger<SepetimController> logger)
        {
            _basketStorageService = basketStorageService;
            _catalogApiService = catalogApiService;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            BasketTotalDto? basket = null;
            try
            {
                basket = await _basketStorageService.GetBasketAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sepet verisi getirilirken hata oluştu.");
                basket = new BasketTotalDto { BasketItems = new() };
            }

            basket ??= new BasketTotalDto { BasketItems = new() };
            basket.BasketItems ??= new();

            // Eksik vitrin fotoğrafı olan ürünler varsa Catalog servisinden dinamik doldur
            if (basket.BasketItems.Any())
            {
                bool basketUpdated = false;
                foreach (var item in basket.BasketItems)
                {
                    if (string.IsNullOrWhiteSpace(item.ImageUrl))
                    {
                        try
                        {
                            var images = await _catalogApiService.GetProductImagesByProductIdAsync(item.ProductId);
                            var mainImg = images?.FirstOrDefault(x => x.IsMain)?.ImageUrl 
                                          ?? images?.FirstOrDefault()?.ImageUrl;

                            if (!string.IsNullOrWhiteSpace(mainImg))
                            {
                                item.ImageUrl = mainImg;
                                basketUpdated = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Ürünün vitrin görseli katalog servisinden alınamadı: {ProductId}", item.ProductId);
                        }
                    }
                }

                if (basketUpdated)
                {
                    try
                    {
                        await _basketStorageService.SaveBasketAsync(basket);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Sepetteki güncellenen ürün görselleri kaydedilemedi.");
                    }
                }
            }

            return View(basket);
        }

        [HttpGet("RemoveItem")]
        [HttpPost("RemoveItem")]
        public async Task<IActionResult> RemoveItem([FromQuery] Guid productId)
        {
            if (productId == Guid.Empty)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var basket = await _basketStorageService.GetBasketAsync();
                if (basket?.BasketItems != null)
                {
                    basket.BasketItems.RemoveAll(x => x.ProductId == productId || x.Id == productId);

                    if (!basket.BasketItems.Any())
                    {
                        basket.BasketType = BasketType.Sale;
                    }

                    await _basketStorageService.SaveBasketAsync(basket);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün sepetten çıkarılırken hata oluştu: {ProductId}", productId);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity(Guid productId, int quantity)
        {
            if (productId == Guid.Empty)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var basket = await _basketStorageService.GetBasketAsync();
                if (basket?.BasketItems != null)
                {
                    var item = basket.BasketItems.FirstOrDefault(x => x.ProductId == productId || x.Id == productId);
                    if (item != null)
                    {
                        item.Quantity = quantity;
                        if (item.Quantity <= 0)
                        {
                            basket.BasketItems.Remove(item);
                        }
                    }

                    if (!basket.BasketItems.Any())
                    {
                        basket.BasketType = BasketType.Sale;
                    }

                    await _basketStorageService.SaveBasketAsync(basket);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sepet adedi güncellenirken hata oluştu: {ProductId}", productId);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Clear")]
        [HttpPost("Clear")]
        public async Task<IActionResult> Clear()
        {
            try
            {
                await _basketStorageService.DeleteBasketAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sepet temizlenirken hata oluştu.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
