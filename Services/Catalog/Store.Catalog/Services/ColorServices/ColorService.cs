using AutoMapper;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Store.Catalog.Dtos.ColorDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;

namespace Store.Catalog.Services.ColorServices
{
    public class ColorService : IColorService
    {
        private readonly IMongoCollection<Color> _colorCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMapper _mapper;
        private readonly ILogger<ColorService> _logger;

        public ColorService(IMapper mapper, IDatabaseSettings databaseSettings, IMongoDatabase database, ILogger<ColorService> logger)
        {
            _colorCollection = database.GetCollection<Color>(databaseSettings.ColorCollectionName);
            _productCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ResultColorDto>> GetAllColorAsync(bool showDeleted = false)
        {
            var filter = showDeleted
                ? FilterDefinition<Color>.Empty
                : Builders<Color>.Filter.Eq(x => x.IsDeleted, false);

            var values = await _colorCollection
                .Find(filter)
                .SortBy(x => x.DisplayOrder)
                .ToListAsync();

            return _mapper.Map<List<ResultColorDto>>(values);
        }

        public async Task<GetByIdColorDto?> GetByIdColorAsync(Guid id)
        {
            var entity = await _colorCollection
                .Find(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            return _mapper.Map<GetByIdColorDto?>(entity);
        }

        public async Task CreateColorAsync(CreateColorDto createColorDto)
        {
            var entity = _mapper.Map<Color>(createColorDto);
            await _colorCollection.InsertOneAsync(entity);
        }

        public async Task UpdateColorAsync(UpdateColorDto updateColorDto)
        {
            var existing = await _colorCollection
                .Find(x => x.Id == updateColorDto.Id)
                .FirstOrDefaultAsync();

            if (existing == null) return;

            // Aktif bir rengi pasife almak istiyorsa ve bu renge bağlı varyant varsa engelle
            if (existing.IsActive && !updateColorDto.IsActive)
            {
                var hasActiveVariant = await _productCollection
                    .Find(x => !x.IsDeleted && x.IsActive &&
                               x.Variants.Any(v => v.ColorId == updateColorDto.Id))
                    .AnyAsync();

                if (hasActiveVariant)
                    throw new Exceptions.BusinessException("Pasife almak istediğiniz Renge bağlı aktif ürün varyantı olduğundan bu kaydı pasife alamazsınız!");
            }

            _mapper.Map(updateColorDto, existing);
            existing.UpdateUserId = UserContextAccessor.GetClientId();
            existing.UpdateDate = DateTime.Now;

            await _colorCollection.ReplaceOneAsync(x => x.Id == existing.Id, existing);
        }

        public async Task DeleteColorAsync(Guid id)
        {
            // Silinmek istenen renge bağlı aktif varyant varsa engelle
            var hasActiveVariant = await _productCollection
                .Find(x => !x.IsDeleted && x.IsActive &&
                           x.Variants.Any(v => v.ColorId == id))
                .AnyAsync();

            if (hasActiveVariant)
                throw new Exceptions.BusinessException("Silmek istediğiniz Renge bağlı aktif ürün varyantı olduğundan bu kaydı silemezsiniz!");

            var update = Builders<Color>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, DateTime.Now);

            await _colorCollection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task RestoreColorAsync(Guid id)
        {
            var update = Builders<Color>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.UpdateUserId, UserContextAccessor.GetClientId())
                .Set(x => x.UpdateDate, DateTime.Now);

            await _colorCollection.UpdateOneAsync(x => x.Id == id, update);
        }
    }
}
