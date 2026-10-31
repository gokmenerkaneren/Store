using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.DtoLayer.BasketDtos;
using Store.DtoLayer.OrderDtos;
using Store.WebUI.Models.CheckoutViewModels;
using Store.WebUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.WebUI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class CheckoutController : Controller
    {
        private readonly IBasketApiService _basketApiService;
        private readonly ICatalogApiService _catalogApiService;
        private readonly IOrderApiService _orderApiService;
        private readonly ILocationApiService _locationApiService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            IBasketApiService basketApiService,
            ICatalogApiService catalogApiService,
            IOrderApiService orderApiService,
            ILocationApiService locationApiService,
            ILogger<CheckoutController> logger)
        {
            _basketApiService = basketApiService;
            _catalogApiService = catalogApiService;
            _orderApiService = orderApiService;
            _locationApiService = locationApiService;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var (basket, items) = await GetHydratedBasketItemsAsync();
            if (basket == null || !items.Any())
            {
                TempData["CartWarning"] = "Sipariş oluşturabilmek için sepetinizde en az 1 ürün bulunmalıdır.";
                return RedirectToAction("Index", "Sepetim");
            }

            var model = new CheckoutViewModel
            {
                Items = items,
                SubTotal = items.Sum(x => x.TotalPrice),
                ShippingFee = 0.00m,
                DiscountAmount = 0.00m
            };

            // Oturum kontrolü ve Guard Clause
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(userIdString, out Guid userId) || userId == Guid.Empty)
            {
                TempData["CartWarning"] = "Siparişi tamamlamak için lütfen giriş yapınız.";
                return RedirectToAction("Index", "Login", new { returnUrl = "/Checkout" });
            }

            // Giriş yapmış kullanıcının bilgilerini form alanlarına doldur
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
            var name = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
            var phone = User.FindFirstValue(ClaimTypes.MobilePhone) ?? User.FindFirstValue("phone_number") ?? User.FindFirstValue("phone");

            if (!string.IsNullOrWhiteSpace(email))
            {
                model.EmailOrPhone = email;
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    model.FirstName = string.Join(" ", parts.Take(parts.Length - 1));
                    model.LastName = parts.Last();
                }
                else
                {
                    model.LastName = name;
                }
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                model.Phone = phone;
            }

            // K8 Koruması: Yalnızca giriş yapmış kullanıcıya ait kayıtlı adresi getir (Asla başkasının adresine fallback yapma)
            try
            {
                var addresses = await _locationApiService.GetUserAddressesAsync(false);
                var userAddr = addresses?.FirstOrDefault(x => x.CustomerId == userId && x.IsDefault) 
                            ?? addresses?.FirstOrDefault(x => x.CustomerId == userId);

                if (userAddr != null)
                {
                    if (!string.IsNullOrWhiteSpace(userAddr.AddressDetail))
                    {
                        model.AddressLine = userAddr.AddressDetail;
                    }
                    if (!string.IsNullOrWhiteSpace(userAddr.PhoneNumber))
                    {
                        model.Phone = userAddr.PhoneNumber;
                    }
                    if (!string.IsNullOrWhiteSpace(userAddr.ContactFullName) && string.IsNullOrWhiteSpace(model.FirstName) && string.IsNullOrWhiteSpace(model.LastName))
                    {
                        var parts = userAddr.ContactFullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 1)
                        {
                            model.FirstName = string.Join(" ", parts.Take(parts.Length - 1));
                            model.LastName = parts.Last();
                        }
                        else
                        {
                            model.LastName = userAddr.ContactFullName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Kullanıcı adresleri getirilirken hata oluştu, form varsayılan bilgilerle doldurulacak.");
            }

            return View(model);
        }

        [HttpPost("")]
        [HttpPost("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            var (basket, items) = await GetHydratedBasketItemsAsync();
            if (basket == null || !items.Any())
            {
                TempData["CartWarning"] = "Sepetiniz boş olduğu için sipariş tamamlanamadı.";
                return RedirectToAction("Index", "Sepetim");
            }

            // Sepet kalemlerini ve toplamlarını model içine yeniden bağla (güvenlik için)
            model.Items = items;
            model.SubTotal = items.Sum(x => x.TotalPrice);
            model.ShippingFee = 0.00m;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(userIdString, out Guid userId) || userId == Guid.Empty)
            {
                TempData["CartWarning"] = "Siparişi tamamlamak için lütfen giriş yapınız.";
                return RedirectToAction("Index", "Login", new { returnUrl = "/Checkout" });
            }

            // İ6: Temel Stok Doğrulama (Catalog & WebUI - Sipariş öncesi stok kontrolü)
            foreach (var item in items)
            {
                try
                {
                    var catalogProduct = await _catalogApiService.GetProductByIdAsync(item.ProductId);
                    if (catalogProduct != null && catalogProduct.Variants != null && catalogProduct.Variants.Any())
                    {
                        var totalStock = catalogProduct.Variants.Sum(v => v.StockQuantity);
                        if (totalStock < item.Quantity)
                        {
                            TempData["CartWarning"] = $"'{item.ProductName}' için yeterli stok bulunamadı (Mevcut stok: {totalStock}, İstenen: {item.Quantity}).";
                            return RedirectToAction("Index", "Sepetim");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Stok kontrolü yapılırken ürün {ProductId} için Catalog servisi uyarısı.", item.ProductId);
                }
            }

            // Sipariş kodu oluştur
            var orderCode = $"SZR-{DateTime.Now:yyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";

            // 1. Order servisine adres ve sipariş kaydını (kalemleriyle birlikte) gönder
            var orderDto = new CreateOrderingDto
            {
                UserId = userId,
                TotalPrice = model.GrandTotal,
                OrderDate = DateTime.Now,
                Items = items.Select(i => new CreateOrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductPrice = i.Price,
                    ProductAmount = i.Quantity,
                    TotalPrice = i.Price * i.Quantity
                }).ToList()
            };

            try
            {
                var addressDto = new CreateOrderAddressDto
                {
                    UserId = userId,
                    Country = model.Country,
                    City = model.City,
                    District = model.District,
                    Region = model.District,
                    Street = model.AddressLine,
                    Detail = string.IsNullOrWhiteSpace(model.Apartment) ? model.AddressLine : $"{model.AddressLine} (Daire/Süit: {model.Apartment})",
                    PostalCode = model.PostalCode ?? string.Empty,
                    Phone = model.Phone ?? model.EmailOrPhone
                };

                await _orderApiService.CreateAddressAsync(addressDto);

                var orderResponse = await _orderApiService.CreateOrderingAsync(orderDto);
                if (!orderResponse.IsSuccessStatusCode)
                {
                    var errorContent = await orderResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Store.Order servisi siparişi kaydedemedi: {StatusCode} - {Error}", orderResponse.StatusCode, errorContent);
                    TempData["CartWarning"] = "Siparişiniz kaydedilirken bir hata oluştu. Sepetiniz korunmuştur, lütfen tekrar deneyiniz.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // İ1 Koruması: Sipariş servisi hata verirse sepet ASLA silinmez, sahte başarı gösterilmez
                _logger.LogError(ex, "Store.Order servisine sipariş kaydedilirken bağlantı hatası oluştu.");
                TempData["CartWarning"] = "Sipariş servisine ulaşılamadı. Sepetiniz korunmuştur, lütfen daha sonra tekrar deneyiniz.";
                return RedirectToAction("Index");
            }

            // 2. YALNIZCA sipariş veritabanında başarıyla oluşturulduktan sonra sepeti temizle
            try
            {
                await _basketApiService.DeleteMyBasketAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Sipariş sonrası sepet temizlenirken hata oluştu.");
            }

            // 3. Sipariş onay modeli hazırla ve TempData'ya aktar
            var fullName = string.IsNullOrWhiteSpace(model.FirstName) 
                ? model.LastName 
                : $"{model.FirstName} {model.LastName}".Trim();

            var fullAddress = $"{model.AddressLine} {model.Apartment}".Trim() + $", {model.District} / {model.City} ({model.Country})";

            var paymentDisplay = model.PaymentMethod switch
            {
                "BankTransfer" => "Havale / EFT",
                "CashOnDelivery" => "Kapıda Nakit / Kredi Kartı",
                _ => "Kredi Kartı (Online Güvenli Ödeme)"
            };

            var orderSuccess = new OrderSuccessViewModel
            {
                OrderCode = orderCode,
                OrderDate = DateTime.Now,
                CustomerEmail = model.EmailOrPhone,
                CustomerFullName = fullName,
                ShippingAddress = fullAddress,
                PaymentMethod = paymentDisplay,
                TotalAmount = model.GrandTotal,
                Items = items
            };

            TempData["OrderSuccess"] = JsonSerializer.Serialize(orderSuccess);

            return RedirectToAction("OrderSuccess");
        }

        [HttpGet("OrderSuccess")]
        public IActionResult OrderSuccess()
        {
            if (TempData["OrderSuccess"] is string json && !string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    var model = JsonSerializer.Deserialize<OrderSuccessViewModel>(json);
                    if (model != null)
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sipariş onay verisi deserialize edilirken hata oluştu.");
                }
            }

            // TempData yoksa veya doğrudan erişildiyse ana sayfaya yönlendir
            return RedirectToAction("Index", "Default");
        }

        #region Private Helpers

        private async Task<(BasketTotalDto? basket, List<CheckoutItemViewModel> items)> GetHydratedBasketItemsAsync()
        {
            BasketTotalDto? basket = null;
            try
            {
                basket = await _basketApiService.GetMyBasketDetailAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme sayfası için sepet getirilirken hata oluştu.");
            }

            if (basket == null || basket.BasketItems == null || !basket.BasketItems.Any())
            {
                return (null, new List<CheckoutItemViewModel>());
            }

            var items = new List<CheckoutItemViewModel>();

            foreach (var item in basket.BasketItems)
            {
                var img = item.ImageUrl;
                if (string.IsNullOrWhiteSpace(img))
                {
                    try
                    {
                        var images = await _catalogApiService.GetProductImagesByProductIdAsync(item.ProductId);
                        img = images?.FirstOrDefault(x => x.IsMain)?.ImageUrl ?? images?.FirstOrDefault()?.ImageUrl;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Ödeme sayfasında ürün görseli katalog servisinden alınamadı: {ProductId}", item.ProductId);
                    }
                }

                items.Add(new CheckoutItemViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName ?? "Ürün",
                    ImageUrl = img,
                    Quantity = item.Quantity > 0 ? item.Quantity : 1,
                    Price = item.Price
                });
            }

            return (basket, items);
        }

        #endregion
    }
}
