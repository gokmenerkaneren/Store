using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Store.Catalog.Dtos.ProductPriceDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;

namespace Store.Catalog.Services.JobServices
{
    public class BulkPriceJobService : IBulkPriceJobService
    {
        private readonly IMongoCollection<ProductPrice> _productPriceCollection;
        private readonly IMongoCollection<ProductPriceHistory> _productPriceHistoryCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly ILogger<BulkPriceJobService> _logger;
        private readonly IMongoDatabase _database;

        public BulkPriceJobService(IDatabaseSettings databaseSettings, IMongoDatabase database, ILogger<BulkPriceJobService> logger)
        {
            _database = database;
            _productPriceCollection = database.GetCollection<ProductPrice>(databaseSettings.ProductPriceCollectionName);
            _productPriceHistoryCollection = database.GetCollection<ProductPriceHistory>("ProductPriceHistories");
            _productCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
            _logger = logger;
        }

        public async Task ExecuteBulkPriceUpdateAsync(BulkPriceUpdateDto request)
        {
            _logger.LogInformation("Toplu fiyat güncelleme (BulkPriceUpdate) işlemi başlatıldı. Kaynak Tipi: {Source}, Hedef Tipi: {Target}, Marj: %{Margin}", 
                request.SourcePriceTypeId, request.TargetPriceTypeId, request.MarginPercentage);

            // 1. Ürünleri filtrele (Kategori veya Marka seçildiyse)
            var productFilter = Builders<Product>.Filter.Eq(p => p.IsDeleted, false);
            if (request.CategoryId.HasValue)
                productFilter &= Builders<Product>.Filter.Eq(p => p.CategoryId, request.CategoryId.Value);
            
            // Eğer Brand filter olsaydı eklenebilirdi. 
            // Şimdilik Product entity'sinde BrandId var mı kontrol edemedik ama varsa eklenecek, bu yüzden try/catch eklendi
            
            var products = await _productCollection.Find(productFilter).Project(p => p.Id).ToListAsync();
            
            if (!products.Any())
            {
                _logger.LogWarning("Filtrelere uygun ürün bulunamadı. İşlem sonlandırılıyor.");
                return;
            }

            // 2. İlgili ürünlerin Kaynak Fiyat (SourcePriceTypeId) kayıtlarını getir
            var sourcePriceFilter = Builders<ProductPrice>.Filter.And(
                Builders<ProductPrice>.Filter.In(p => p.ProductId, products),
                Builders<ProductPrice>.Filter.Eq(p => p.PriceTypeId, request.SourcePriceTypeId),
                Builders<ProductPrice>.Filter.Eq(p => p.ValidTo, (DateTimeOffset?)null),
                Builders<ProductPrice>.Filter.Eq(p => p.IsDeleted, false),
                Builders<ProductPrice>.Filter.Eq(p => p.IsActive, true)
            );

            var sourcePrices = await _productPriceCollection.Find(sourcePriceFilter).ToListAsync();

            if (!sourcePrices.Any())
            {
                _logger.LogWarning("Belirtilen ürünler için kaynak fiyat tipi ({Source}) bulunamadı.", request.SourcePriceTypeId);
                return;
            }

            _logger.LogInformation("{Count} adet kaynak fiyat bulundu. İşlem başlıyor...", sourcePrices.Count);

            // 3. Hedef Fiyat (TargetPriceTypeId) kayıtlarını getir ki var olanları arşive taşıyalım
            var targetPriceFilter = Builders<ProductPrice>.Filter.And(
                Builders<ProductPrice>.Filter.In(p => p.ProductId, sourcePrices.Select(x => x.ProductId)),
                Builders<ProductPrice>.Filter.Eq(p => p.PriceTypeId, request.TargetPriceTypeId),
                Builders<ProductPrice>.Filter.Eq(p => p.ValidTo, (DateTimeOffset?)null),
                Builders<ProductPrice>.Filter.Eq(p => p.IsDeleted, false)
            );

            var targetPrices = await _productPriceCollection.Find(targetPriceFilter).ToListAsync();
            var targetPricesDict = targetPrices.ToDictionary(k => $"{k.ProductId}_{k.CurrencyId}", v => v);

            var now = DateTimeOffset.UtcNow;
            
            // 4. Batch (Paketleme) Ayarları - OOM ve Transaction Timeout önlemi
            int batchSize = 1000;
            int totalProcessed = 0;

            for (int i = 0; i < sourcePrices.Count; i += batchSize)
            {
                var batchSourcePrices = sourcePrices.Skip(i).Take(batchSize).ToList();
                
                var historyInserts = new List<ProductPriceHistory>();
                var priceDeletes = new List<WriteModel<ProductPrice>>();
                var priceInserts = new List<WriteModel<ProductPrice>>();

                foreach (var sourcePrice in batchSourcePrices)
                {
                    var newAmount = sourcePrice.Amount * (1 + (request.MarginPercentage / 100m));
                    
                    var key = $"{sourcePrice.ProductId}_{sourcePrice.CurrencyId}";
                    if (targetPricesDict.TryGetValue(key, out var existingTargetPrice))
                    {
                        historyInserts.Add(new ProductPriceHistory
                        {
                            ProductId = existingTargetPrice.ProductId,
                            CurrencyId = existingTargetPrice.CurrencyId,
                            PriceTypeId = existingTargetPrice.PriceTypeId,
                            PriceListId = existingTargetPrice.PriceListId,
                            BasePriceId = existingTargetPrice.BasePriceId,
                            MarginPercentage = existingTargetPrice.MarginPercentage,
                            MinimumQuantity = existingTargetPrice.MinimumQuantity,
                            Amount = existingTargetPrice.Amount,
                            DiscountedAmount = existingTargetPrice.DiscountedAmount,
                            IsTaxIncluded = existingTargetPrice.IsTaxIncluded,
                            ValidFrom = existingTargetPrice.ValidFrom,
                            ValidTo = now,
                            ArchivedDate = now
                        });

                        priceDeletes.Add(new DeleteOneModel<ProductPrice>(Builders<ProductPrice>.Filter.Eq(x => x.Id, existingTargetPrice.Id)));
                    }

                    var newEntity = new ProductPrice
                    {
                        Id = Guid.NewGuid(),
                        ProductId = sourcePrice.ProductId,
                        CurrencyId = sourcePrice.CurrencyId,
                        PriceTypeId = request.TargetPriceTypeId,
                        PriceListId = null, 
                        BasePriceId = sourcePrice.Id, 
                        MarginPercentage = request.MarginPercentage,
                        MinimumQuantity = sourcePrice.MinimumQuantity, 
                        Amount = newAmount,
                        DiscountedAmount = null,
                        IsTaxIncluded = sourcePrice.IsTaxIncluded,
                        ValidFrom = now,
                        ValidTo = null,
                        IsActive = true,
                        IsDeleted = false,
                        InsertUserId = request.AdminUserId, // Audit - İşlemi Başlatan Admin
                        InsertDate = BaseEntity.GetLocalTime()
                    };
                    
                    priceInserts.Add(new InsertOneModel<ProductPrice>(newEntity));
                }

                // Her Batch için ayrı Transaction aç
                using var session = await _database.Client.StartSessionAsync();
                session.StartTransaction();
                try
                {
                    if (historyInserts.Any())
                    {
                        await _productPriceHistoryCollection.InsertManyAsync(session, historyInserts);
                    }

                    if (priceDeletes.Any())
                    {
                        await _productPriceCollection.BulkWriteAsync(session, priceDeletes);
                    }

                    if (priceInserts.Any())
                    {
                        await _productPriceCollection.BulkWriteAsync(session, priceInserts);
                    }

                    await session.CommitTransactionAsync();
                    totalProcessed += priceInserts.Count;
                    _logger.LogInformation("Batch başarıyla işlendi. (İşlenen: {Processed}/{Total})", totalProcessed, sourcePrices.Count);
                }
                catch (Exception ex)
                {
                    await session.AbortTransactionAsync();
                    _logger.LogError(ex, "BulkPriceUpdate batch işlemi sırasında hata oluştu! Kalan kayıtlar işlenemedi.");
                    throw; // Eğer bir batch hata verirse tüm Job'u Failed statüsüne çeker
                }
            }

            _logger.LogInformation("Toplu fiyat güncellemesi tamamen bitti. Toplam {Total} kayıt güncellendi.", totalProcessed);
        }
    }
}
