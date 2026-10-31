using AutoMapper;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Store.Catalog.Dtos.SizeDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;

namespace Store.Catalog.Services.SizeServices
{
    public class SizeService : ISizeService
    {
        private readonly IMongoCollection<Size> _sizeCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMapper _mapper;
        private readonly ILogger<SizeService> _logger;

        public SizeService(IMapper mapper, IDatabaseSettings databaseSettings, IMongoDatabase database, ILogger<SizeService> logger)
        {
            _sizeCollection = database.GetCollection<Size>(databaseSettings.SizeCollectionName);
            _productCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ResultSizeDto>> GetAllSizeAsync(bool showDeleted = false)
        {
            var filter = showDeleted
                ? FilterDefinition<Size>.Empty
                : Builders<Size>.Filter.Eq(x => x.IsDeleted, false);

            var values = await _sizeCollection
                .Find(filter)
                .SortBy(x => x.SizeType)
                .ThenBy(x => x.DisplayOrder)
                .ToListAsync();

            return _mapper.Map<List<ResultSizeDto>>(values);
        }

        public async Task<GetByIdSizeDto?> GetByIdSizeAsync(Guid id)
        {
            var entity = await _sizeCollection
                .Find(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            return _mapper.Map<GetByIdSizeDto?>(entity);
        }

        public async Task CreateSizeAsync(CreateSizeDto createSizeDto)
        {
            var entity = _mapper.Map<Size>(createSizeDto);
            await _sizeCollection.InsertOneAsync(entity);
        }

        public async Task UpdateSizeAsync(UpdateSizeDto updateSizeDto)
        {
            var existing = await _sizeCollection
                .Find(x => x.Id == updateSizeDto.Id)
                .FirstOrDefaultAsync();

            if (existing == null) return;

            // Aktif bir bedeni pasife almak istiyorsa ve bu bedene bağlı varyant varsa engelle
            if (existing.IsActive && !updateSizeDto.IsActive)
            {
                var hasActiveVariant = await _productCollection
                    .Find(x => !x.IsDeleted && x.IsActive &&
                               x.Variants.Any(v => v.SizeId == updateSizeDto.Id))
                    .AnyAsync();

                if (hasActiveVariant)
                    throw new Exceptions.BusinessException("Pasife almak istediğiniz Bedene bağlı aktif ürün varyantı olduğundan bu kaydı pasife alamazsınız!");
            }

            _mapper.Map(updateSizeDto, existing);
            existing.UpdateUserId = UserContextAccessor.GetClientId();
            existing.UpdateDate = DateTime.Now;

            await _sizeCollection.ReplaceOneAsync(x => x.Id == existing.Id, existing);
        }

        public async Task DeleteSizeAsync(Guid id)
        {
            // Silinmek istenen bedene bağlı aktif varyant varsa engelle
            var hasActiveVariant = await _productCollection
                .Find(x => !x.IsDeleted && x.IsActive &&
                           x.Variants.Any(v => v.SizeId == id))
                .AnyAsync();

            if (hasActiveVariant)
                throw new Exceptions.BusinessException("Silmek istediğiniz Bedene bağlı aktif ürün varyantı olduğundan bu kaydı silemezsiniz!");

            var update = Builders<Size>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, DateTime.Now);

            await _sizeCollection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task RestoreSizeAsync(Guid id)
        {
            var update = Builders<Size>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.UpdateUserId, UserContextAccessor.GetClientId())
                .Set(x => x.UpdateDate, DateTime.Now);

            await _sizeCollection.UpdateOneAsync(x => x.Id == id, update);
        }
    }
}
