using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Store.Catalog.Dtos.PricingDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using System.Text.Json;

namespace Store.Catalog.Services.PricingServices
{
    public class PricingService : IPricingService
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<PriceType> _priceTypeCollection;
        private readonly IDistributedCache _cache;
        private readonly ILogger<PricingService> _logger;

        public PricingService(
            IMongoDatabase database, 
            IDatabaseSettings databaseSettings,
            IDistributedCache cache, 
            ILogger<PricingService> logger)
        {
            _productCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
            // Assuming PriceTypeCollectionName exists or just use a magic string/hardcoded collection for now
            _priceTypeCollection = database.GetCollection<PriceType>("PriceTypes");
            _cache = cache;
            _logger = logger;
        }

        public async Task<CalculatedPriceDto> CalculatePriceAsync(Guid productId, Guid? dealerId = null)
        {
            var product = await _productCollection.Find(x => x.Id == productId).FirstOrDefaultAsync();
            if (product == null)
                throw new Exception("Product not found");

            // Get the best or default active price (for MVP, we just take the first active price)
            var activePrice = product.ActivePrices?.FirstOrDefault(p => p.IsActive && !p.IsDeleted && (p.ValidTo == null || p.ValidTo > DateTimeOffset.UtcNow));
            
            if (activePrice == null)
            {
                throw new Exception("No active price found for this product.");
            }

            var priceType = await _priceTypeCollection.Find(x => x.Id == activePrice.PriceTypeId).FirstOrDefaultAsync();
            var priceTypeName = priceType?.Name ?? "Standard";

            decimal basePrice = activePrice.Amount;
            decimal finalPrice = activePrice.DiscountedAmount ?? basePrice;
            decimal discountRate = 0;

            // B2B İskonto Kontrolü (Redis Cache üzerinden)
            if (dealerId.HasValue)
            {
                try
                {
                    string cacheKey = $"DealerDiscount_{dealerId.Value}";
                    var cachedDiscountStr = await _cache.GetStringAsync(cacheKey);

                    if (!string.IsNullOrEmpty(cachedDiscountStr) && decimal.TryParse(cachedDiscountStr, out decimal dealerDiscountRate))
                    {
                        discountRate = dealerDiscountRate;
                        // İskontoyu uygula (örneğin %15 indirim => fiyat * 0.85)
                        finalPrice = basePrice - (basePrice * (discountRate / 100));
                        _logger.LogInformation($"B2B Discount of {discountRate}% applied for Dealer {dealerId.Value}. Original: {basePrice}, Final: {finalPrice}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to fetch dealer discount from Redis for DealerId: {dealerId.Value}");
                }
            }

            return new CalculatedPriceDto
            {
                ProductId = productId,
                BasePrice = basePrice,
                FinalPrice = finalPrice,
                DiscountRateApplied = discountRate,
                DiscountAmount = basePrice - finalPrice,
                CurrencyId = activePrice.CurrencyId,
                PriceTypeId = activePrice.PriceTypeId,
                PriceTypeName = priceTypeName,
                IsTaxIncluded = activePrice.IsTaxIncluded
            };
        }
    }
}
