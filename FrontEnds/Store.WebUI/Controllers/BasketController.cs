using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.BasketDtos;
using Store.WebUI.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Store.WebUI.Controllers
{
    [AllowAnonymous]
    [Route("Basket")]
    public class BasketController : Controller
    {
        private readonly IBasketStorageService _basketStorageService;
        private readonly ILogger<BasketController> _logger;

        public BasketController(
            IBasketStorageService basketStorageService,
            ILogger<BasketController> logger)
        {
            _basketStorageService = basketStorageService;
            _logger = logger;
        }

        [HttpGet("GetBasket")]
        public async Task<IActionResult> GetBasket()
        {
            try
            {
                var basket = await _basketStorageService.GetBasketAsync();
                basket ??= new BasketTotalDto { BasketItems = new() };
                return Ok(new { success = true, data = basket });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBasket sırasında hata oluştu.");
                return Ok(new { success = true, data = new BasketTotalDto { BasketItems = new() }, isOffline = true });
            }
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToBasketRequest request)
        {
            if (request == null || request.ProductId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Geçersiz ürün bilgisi." });
            }

            try
            {
                var basket = await _basketStorageService.GetBasketAsync();
                basket ??= new BasketTotalDto();
                basket.BasketItems ??= new();

                var targetType = request.IsRental ? BasketType.Rental : BasketType.Sale;

                // Satış / Kiralama çakışma (Conflict) kontrolü
                if (basket.BasketItems.Any() && basket.BasketType != targetType)
                {
                    if (!request.ClearAndAdd)
                    {
                        var currentTypeName = basket.BasketType == BasketType.Sale ? "Satış" : "Kiralama";
                        var newTypeName = targetType == BasketType.Sale ? "Satış" : "Kiralama";

                        return StatusCode(409, new
                        {
                            success = false,
                            isConflict = true,
                            currentType = basket.BasketType.ToString(),
                            newType = targetType.ToString(),
                            message = $"Sepetinizde {currentTypeName} siparişi bulunmaktadır. {newTypeName} ürünü ekleyebilmek için mevcut sepetinizi boşaltmanız gerekir. Sepeti boşaltıp bu ürünü eklemek istiyor musunuz?"
                        });
                    }

                    // Kullanıcı onayı geldiyse sepeti temizle ve tipi güncelle
                    basket.BasketItems.Clear();
                    basket.BasketType = targetType;
                }
                else if (!basket.BasketItems.Any())
                {
                    basket.BasketType = targetType;
                }

                // Aynı ürün sepette zaten varsa miktarını artır
                var existingItem = basket.BasketItems.FirstOrDefault(x => x.ProductId == request.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += (request.Quantity > 0 ? request.Quantity : 1);
                }
                else
                {
                    basket.BasketItems.Add(new BasketItemDto
                    {
                        Id = Guid.NewGuid(),
                        ProductId = request.ProductId,
                        ProductName = request.ProductName,
                        ImageUrl = request.ImageUrl,
                        Quantity = request.Quantity > 0 ? request.Quantity : 1,
                        Price = request.Price,
                        IsRental = request.IsRental,
                        RentalStartDate = request.RentalStartDate,
                        RentalEndDate = request.RentalEndDate
                    });
                }

                await _basketStorageService.SaveBasketAsync(basket);
                return Ok(new { success = true, data = basket, message = "Ürün sepete başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddToCart sırasında hata oluştu.");
                return StatusCode(500, new { success = false, message = "Sepete ekleme sırasında bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateBasketQuantityRequest request)
        {
            if (request == null || request.ProductId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Geçersiz istek." });
            }

            try
            {
                var basket = await _basketStorageService.GetBasketAsync();
                if (basket?.BasketItems == null || !basket.BasketItems.Any())
                {
                    return Ok(new { success = true, data = basket });
                }

                var item = basket.BasketItems.FirstOrDefault(x => x.ProductId == request.ProductId || x.Id == request.ProductId);
                if (item != null)
                {
                    if (request.Change.HasValue)
                    {
                        item.Quantity += request.Change.Value;
                    }
                    else if (request.Quantity.HasValue)
                    {
                        item.Quantity = request.Quantity.Value;
                    }

                    // Miktar 0 veya altına indiyse ürünü sepetten çıkar
                    if (item.Quantity <= 0)
                    {
                        basket.BasketItems.Remove(item);
                    }
                }

                // Sepet tamamen boşaldıysa varsayılan tipe dön
                if (!basket.BasketItems.Any())
                {
                    basket.BasketType = BasketType.Sale;
                }

                await _basketStorageService.SaveBasketAsync(basket);
                return Ok(new { success = true, data = basket });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateQuantity sırasında hata oluştu.");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("RemoveItem")]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveBasketItemRequest request)
        {
            if (request == null || request.ProductId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Geçersiz ürün." });
            }

            try
            {
                var basket = await _basketStorageService.GetBasketAsync();
                if (basket?.BasketItems != null)
                {
                    basket.BasketItems.RemoveAll(x => x.ProductId == request.ProductId || x.Id == request.ProductId);

                    if (!basket.BasketItems.Any())
                    {
                        basket.BasketType = BasketType.Sale;
                    }

                    await _basketStorageService.SaveBasketAsync(basket);
                }

                return Ok(new { success = true, data = basket, message = "Ürün sepetten silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveItem sırasında hata oluştu.");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ClearBasket")]
        public async Task<IActionResult> ClearBasket()
        {
            try
            {
                await _basketStorageService.DeleteBasketAsync();
                return Ok(new { success = true, message = "Sepet temizlendi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClearBasket sırasında hata oluştu.");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class AddToBasketRequest
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public bool IsRental { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public bool ClearAndAdd { get; set; } = false;
    }

    public class UpdateBasketQuantityRequest
    {
        public Guid ProductId { get; set; }
        public int? Change { get; set; } // +1 veya -1
        public int? Quantity { get; set; } // doğrudan miktar
    }

    public class RemoveBasketItemRequest
    {
        public Guid ProductId { get; set; }
    }
}
