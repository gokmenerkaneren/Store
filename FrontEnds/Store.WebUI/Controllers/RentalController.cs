using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Models.Rental;
using Store.WebUI.Services.Interfaces;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Store.WebUI.Controllers
{
    [Authorize]
    public class RentalController : Controller
    {
        private readonly IRentalService _rentalService;
        private readonly ICatalogApiService _catalogApiService;
        private readonly ILogger<RentalController> _logger;

        public RentalController(
            IRentalService rentalService, 
            ICatalogApiService catalogApiService,
            ILogger<RentalController> logger)
        {
            _rentalService = rentalService;
            _catalogApiService = catalogApiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string productId, decimal? price = null)
        {
            var model = new CreateRentalViewModel();
            if (!string.IsNullOrEmpty(productId))
            {
                decimal verifiedPrice = price ?? 0;

                // Güvenlik K7: Birim fiyat istemcinin query parametresinden doğrudan güvenilmez;
                // Katalog servisinden doğrulanmış fiyat sorgulanır.
                if (Guid.TryParse(productId, out var productGuid))
                {
                    try
                    {
                        var prices = await _catalogApiService.GetProductPricesByProductIdAsync(productGuid);
                        var activePrice = prices?.FirstOrDefault(p => p.IsActive && (!p.ValidTo.HasValue || p.ValidTo > DateTimeOffset.UtcNow));
                        if (activePrice != null)
                        {
                            verifiedPrice = activePrice.DiscountedAmount ?? activePrice.Amount;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Ürün ({ProductId}) için katalog fiyatı sorgulanırken hata oluştu; mevcut fiyat parametresi kullanılıyor.", productId);
                    }
                }

                model.Items.Add(new RentalItemViewModel 
                { 
                    ProductId = productId, 
                    Quantity = 1, 
                    UnitPrice = verifiedPrice 
                });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CreateRentalViewModel model)
        {
            try
            {
                // Güvenlik K7: Gönderilen formdaki birim fiyatları katalog fiyat listesiyle doğrulanır
                if (model.Items != null && model.Items.Any())
                {
                    foreach (var item in model.Items)
                    {
                        if (Guid.TryParse(item.ProductId, out var productGuid))
                        {
                            try
                            {
                                var prices = await _catalogApiService.GetProductPricesByProductIdAsync(productGuid);
                                var activePrice = prices?.FirstOrDefault(p => p.IsActive && (!p.ValidTo.HasValue || p.ValidTo > DateTimeOffset.UtcNow));
                                if (activePrice != null)
                                {
                                    // İstemcinin manipüle ettiği fiyatı ez, katalogdaki gerçek fiyatı yaz
                                    item.UnitPrice = activePrice.DiscountedAmount ?? activePrice.Amount;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Katalog fiyat doğrulaması yapılamadı ({ProductId}), mevcut değer korunuyor.", item.ProductId);
                            }
                        }

                        if (item.UnitPrice <= 0)
                        {
                            TempData["ErrorMessage"] = "Ürün için geçerli bir birim fiyat bulunamadı.";
                            return View(model);
                        }
                    }
                }

                await _rentalService.CreateRentalOrderAsync(model);
                TempData["SuccessMessage"] = "Kiralama talebiniz başarıyla alındı!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Seçili tarihlerde ürün müsait değil veya hata oluştu: " + ex.Message;
                return View(model);
            }
        }
    }
}
