using AutoMapper;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Store.Catalog.Dtos.ProductImageDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Services.ProductImageServices
{
    public class ProductImageService : IProductImageService
    {
        private readonly IMongoCollection<ProductImage> _productImageCollection;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductImageService> _logger;

        public ProductImageService(IMapper mapper, IDatabaseSettings databaseSettings, IMongoDatabase database, ILogger<ProductImageService> logger)
        {
            _productImageCollection = database.GetCollection<ProductImage>(databaseSettings.ProductImageCollectionName);
            _mapper = mapper;
            _logger = logger;
        }

        public async Task CreateProductImageAsync(CreateProductImageDto createProductImageDto)
        {
            if (createProductImageDto.IsMain)
            {
                var update = Builders<ProductImage>.Update.Set(x => x.IsMain, false);
                await _productImageCollection.UpdateManyAsync(x => x.ProductId == createProductImageDto.ProductId && x.IsMain == true, update);
            }

            var entity = _mapper.Map<ProductImage>(createProductImageDto);
            await _productImageCollection.InsertOneAsync(entity);
        }

        public async Task DeleteProductImageAsync(Guid id)
        {
            var guidId = id;
            var existingImage = await _productImageCollection.Find(x => x.Id == guidId).FirstOrDefaultAsync();
            if (existingImage != null)
            {
                // Soft delete: set IsDeleted = true, set DeleteUserId and DeleteDate (UTC)
                var update = Builders<ProductImage>.Update
                    .Set(x => x.IsDeleted, true)
                    .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                    .Set(x => x.DeleteDate, BaseEntity.GetLocalTime());

                if (existingImage.IsMain)
                {
                    update = update.Set(x => x.IsMain, false);
                }

                await _productImageCollection.UpdateOneAsync(x => x.Id == guidId, update);

                // Eger silinen resim vitrin resmiyse, kalanlardan ilkini vitrin resmi yap
                if (existingImage.IsMain)
                {
                    var nextImage = await _productImageCollection.Find(x => x.ProductId == existingImage.ProductId && x.Id != guidId && !x.IsDeleted).FirstOrDefaultAsync();
                    if (nextImage != null)
                    {
                        var setMainUpdate = Builders<ProductImage>.Update.Set(x => x.IsMain, true);
                        await _productImageCollection.UpdateOneAsync(x => x.Id == nextImage.Id, setMainUpdate);
                    }
                }
            }
        }

        public async Task<List<ResultProductImageDto>> GetAllProductImageAsync(bool showDeleted = false)
        {
            // If showDeleted is true, fetch all. Otherwise, only non-deleted ones.
            var filter = showDeleted ? FilterDefinition<ProductImage>.Empty : Builders<ProductImage>.Filter.Eq(x => x.IsDeleted, false);
            var values = await _productImageCollection.Find(filter).ToListAsync();
            return _mapper.Map<List<ResultProductImageDto>>(values);
        }

        public async Task<GetByIdProductImageDto?> GetByIdProductImageAsync(Guid id)
        {
            var guidId = id;
            var entity = await _productImageCollection.Find<ProductImage>(x => x.Id == guidId && !x.IsDeleted).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdProductImageDto?>(entity);
        }

        public async Task<List<ResultProductImageDto>> GetByProductIdProductImageAsync(Guid productId)
        {
            var guidProductId = productId;
            var values = await _productImageCollection.Find<ProductImage>(x => x.ProductId == guidProductId && !x.IsDeleted).ToListAsync();
            return _mapper.Map<List<ResultProductImageDto>>(values);
        }

        public async Task UpdateProductImageAsync(UpdateProductImageDto updateProductImageDto)
        {
            var guidId = updateProductImageDto.Id;
            var existing = await _productImageCollection.Find<ProductImage>(x => x.Id == guidId).FirstOrDefaultAsync();
            if (existing == null)
            {
                return;
            }

            _mapper.Map(updateProductImageDto, existing);
            existing.UpdateUserId = Guid.Parse("00000002-0002-0002-0002-000000000002");
            existing.UpdateDate = BaseEntity.GetLocalTime();

            await _productImageCollection.ReplaceOneAsync(x => x.Id == existing.Id, existing);
        }
    }
}


