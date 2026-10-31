using AutoMapper;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Store.Catalog.Dtos.ProductPriceDtos;
using Store.Catalog.Entities;
using Store.Shared.Entities;
using Store.Shared.Enums;
using Store.Catalog.Settings;

namespace Store.Catalog.Services.ProductPriceServices
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IMongoCollection<ProductPrice> _productPriceCollection;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductPriceService> _logger;

        public ProductPriceService(IMapper mapper, IDatabaseSettings databaseSettings, IMongoDatabase database, ILogger<ProductPriceService> logger)
        {
            _productPriceCollection = database.GetCollection<ProductPrice>(databaseSettings.ProductPriceCollectionName);
            _mapper = mapper;
            _logger = logger;

            // Partial Unique Index: Aynı ürün, para birimi ve fiyat tipi için sadece 1 tane aktif (ValidTo == null) fiyat olabilir.
            CreateProductPriceIndex();
        }

        private void CreateProductPriceIndex()
        {
            try
            {
                var indexKeysDefinition = Builders<ProductPrice>.IndexKeys
                    .Ascending(p => p.ProductId)
                    .Ascending(p => p.CurrencyId)
                    .Ascending(p => p.PriceTypeId);
                var indexOptions = new CreateIndexOptions<ProductPrice>
                { 
                    Unique = true, 
                    PartialFilterExpression = Builders<ProductPrice>.Filter.Eq(p => p.ValidTo, (DateTimeOffset?)null)
                };
                var indexModel = new CreateIndexModel<ProductPrice>(indexKeysDefinition, indexOptions);
                _productPriceCollection.Indexes.CreateOne(indexModel);
            }
            catch (MongoCommandException ex) when (ex.Message.Contains("E11000 duplicate key error"))
            {
                try
                {
                    try 
                    {
                        _productPriceCollection.Indexes.DropOne("ProductId_1_CurrencyId_1_PriceTypeId_1");
                    }
                    catch (MongoCommandException dropEx) when (dropEx.Message.Contains("index not found"))
                    {
                        _logger.LogWarning("Silinmek istenen eski index bulunamadı, yenisi oluşturulmaya devam edilecek.");
                    }
                    _logger.LogWarning("Eski index silindi (veya zaten yoktu). Yeni index oluşturulmaya çalışılıyor...");

                    var indexKeysDefinition = Builders<ProductPrice>.IndexKeys
                        .Ascending(p => p.ProductId)
                        .Ascending(p => p.CurrencyId)
                        .Ascending(p => p.PriceTypeId);
                    var indexOptions = new CreateIndexOptions<ProductPrice>
                    { 
                        Unique = true, 
                        PartialFilterExpression = Builders<ProductPrice>.Filter.Eq(p => p.ValidTo, (DateTimeOffset?)null)
                    };
                    var indexModel = new CreateIndexModel<ProductPrice>(indexKeysDefinition, indexOptions);
                    _productPriceCollection.Indexes.CreateOne(indexModel);
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, 
                        "HATA: ProductPrice koleksiyonunda ValidTo=null olan ve aynı " +
                        "ProductId+CurrencyId+PriceTypeId kombinasyonunda birden fazla kayıt vardır. " +
                        "Lütfen temizleme scripti çalıştırın: Scripts/MongoDB/CleanupDuplicateProductPrices.js");
                    throw;
                }
            }
        }

        private DateTimeOffset? NormalizeDate(DateTimeOffset? dt)
        {
            if (dt == null || dt == DateTimeOffset.MinValue) return null;
            // Always treat as UTC midnight to avoid timezone shifts
            return new DateTimeOffset(dt.Value.Date, TimeSpan.Zero);
        }

        public async Task CreateProductPriceAsync(CreateProductPriceDto createProductPriceDto)
        {
            var guidProductId = createProductPriceDto.ProductId;
            var newStartDate = createProductPriceDto.ValidFrom ?? DateTimeOffset.Now;
            var newEndDate = createProductPriceDto.ValidTo;

            // Aynı ürün, aynı fiyat tipi ve aynı para birimindeki aktif kayıtları bul
            var filter = Builders<ProductPrice>.Filter.And(
                Builders<ProductPrice>.Filter.Eq(x => x.ProductId, guidProductId),
                Builders<ProductPrice>.Filter.Eq(x => x.PriceTypeId, createProductPriceDto.PriceTypeId),
                Builders<ProductPrice>.Filter.Eq(x => x.CurrencyId, createProductPriceDto.CurrencyId),
                Builders<ProductPrice>.Filter.Eq(x => x.IsActive, true),
                Builders<ProductPrice>.Filter.Eq(x => x.IsDeleted, false)
            );

            var activePrices = await _productPriceCollection.Find(filter).ToListAsync();

            foreach (var activePrice in activePrices)
            {
                var existingStart = activePrice.ValidFrom ?? DateTimeOffset.MinValue;
                var existingEnd = activePrice.ValidTo ?? DateTimeOffset.MaxValue;

                var checkStart = newStartDate;
                var checkEnd = newEndDate ?? DateTimeOffset.MaxValue;

                // Overlap mantığı: (Start1 < End2) && (Start2 < End1)
                if (existingStart < checkEnd && checkStart < existingEnd)
                {
                    throw new InvalidOperationException("Belirtilen tarih aralığında, aynı fiyat tipi ve para birimi için aktif bir fiyat zaten tanımlanmıştır. Lütfen tarihleri kontrol ediniz.");
                }
            }

            createProductPriceDto.ValidFrom = newStartDate;
            var entity = _mapper.Map<ProductPrice>(createProductPriceDto);
            await _productPriceCollection.InsertOneAsync(entity);
        }

        public async Task DeleteProductPriceAsync(Guid id)
        {
            var guidId = id;
            // Soft delete: set IsDeleted = true, set IsActive = false, set DeleteUserId and DeleteDate (UTC)
            var update = Builders<ProductPrice>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.IsActive, false)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, BaseEntity.GetLocalTime());

            await _productPriceCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task RestoreProductPriceAsync(Guid id)
        {
            var guidId = id;
            var update = Builders<ProductPrice>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.IsActive, true)
                .Set(x => x.DeleteUserId, (Guid?)null)
                .Set(x => x.DeleteDate, (DateTimeOffset?)null);

            await _productPriceCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task<List<ResultProductPriceDto>> GetAllProductPriceAsync(bool showDeleted = false)
        {
            // If showDeleted is true, fetch all. Otherwise, only non-deleted ones.
            var filter = showDeleted ? FilterDefinition<ProductPrice>.Empty : Builders<ProductPrice>.Filter.Eq(x => x.IsDeleted, false);
            var values = await _productPriceCollection.Find(filter).ToListAsync();
            return _mapper.Map<List<ResultProductPriceDto>>(values);
        }

        public async Task<GetByIdProductPriceDto?> GetByIdProductPriceAsync(Guid id)
        {
            var guidId = id;
            var entity = await _productPriceCollection.Find<ProductPrice>(x => x.Id == guidId && !x.IsDeleted).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdProductPriceDto?>(entity);
        }

        public async Task<List<ResultProductPriceDto>> GetByProductIdProductPriceAsync(Guid productId, bool showDeleted = false)
        {
            var guidProductId = productId;
            var filter = showDeleted 
                ? Builders<ProductPrice>.Filter.Eq(x => x.ProductId, guidProductId)
                : Builders<ProductPrice>.Filter.And(
                    Builders<ProductPrice>.Filter.Eq(x => x.ProductId, guidProductId),
                    Builders<ProductPrice>.Filter.Eq(x => x.IsDeleted, false)
                );

            var values = await _productPriceCollection.Find(filter).ToListAsync();
            return _mapper.Map<List<ResultProductPriceDto>>(values);
        }

        public async Task UpdateProductPriceAsync(UpdateProductPriceDto updateProductPriceDto)
        {
            var guidId = updateProductPriceDto.Id;
            var existing = await _productPriceCollection.Find<ProductPrice>(x => x.Id == guidId).FirstOrDefaultAsync();
            if (existing == null)
            {
                return;
            }

            using var session = await _productPriceCollection.Database.Client.StartSessionAsync();
            session.StartTransaction();
            try
            {
                // 1. Eski fiyatı arşiv tablosuna taşı (ValidTo değerini kapatarak)
                var historyCollection = _productPriceCollection.Database.GetCollection<ProductPriceHistory>("ProductPriceHistories");
                var historyEntity = new ProductPriceHistory
                {
                    ProductId = existing.ProductId,
                    CurrencyId = existing.CurrencyId,
                    PriceTypeId = existing.PriceTypeId,
                    PriceListId = existing.PriceListId,
                    BasePriceId = existing.BasePriceId,
                    MarginPercentage = existing.MarginPercentage,
                    MinimumQuantity = existing.MinimumQuantity,
                    Amount = existing.Amount,
                    DiscountedAmount = existing.DiscountedAmount,
                    IsTaxIncluded = existing.IsTaxIncluded,
                    ValidFrom = existing.ValidFrom,
                    ValidTo = DateTimeOffset.UtcNow,
                    ArchivedDate = DateTimeOffset.UtcNow
                };
                await historyCollection.InsertOneAsync(session, historyEntity);

                // 2. Eski aktif fiyatı sil (Aktif tablodan kaldır)
                await _productPriceCollection.DeleteOneAsync(session, x => x.Id == existing.Id);

                // 3. Yeni fiyatı aktif tabloya ekle
                var newEntity = _mapper.Map<ProductPrice>(updateProductPriceDto);
                newEntity.Id = Guid.NewGuid(); // Yeni bir kayıt gibi insert edeceğiz
                newEntity.ValidFrom = DateTimeOffset.UtcNow;
                newEntity.ValidTo = null; // Aktif olduğunu gösterir
                newEntity.UpdateUserId = Guid.Parse("00000002-0002-0002-0002-000000000002");
                newEntity.UpdateDate = DateTime.Now;

                await _productPriceCollection.InsertOneAsync(session, newEntity);

                await session.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                _logger.LogError(ex, "Transaction aborted during UpdateProductPriceAsync");
                throw;
            }
        }
    }
}