using AutoMapper;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Store.Catalog.Dtos.BrandDtos;
using Store.Catalog.Entities;
using Store.Catalog.Services.CacheServices;
using Store.Catalog.Settings;

namespace Store.Catalog.Services.BrandServices
{
    public class BrandService : IBrandService
    {
        private readonly IMongoCollection<Brand> _brandCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMapper _mapper;
        private readonly ILogger<BrandService> _logger;
        private readonly ICatalogCacheService _cacheService;

        private const string CachePrefix = "catalog:brands:";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public BrandService(IMapper mapper, IDatabaseSettings databaseSettings, IMongoDatabase database, ILogger<BrandService> logger, ICatalogCacheService cacheService)
        {
            _brandCollection = database.GetCollection<Brand>(databaseSettings.BrandCollectionName);
            _productCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task CreateBrandAsync(CreateBrandDto createBrandDto)
        {
            var entity = _mapper.Map<Brand>(createBrandDto);
            await _brandCollection.InsertOneAsync(entity);
            await _cacheService.RemoveByPrefixAsync(CachePrefix);
        }

        public async Task DeleteBrandAsync(Guid id)
        {
            var guidId = id;
            var hasActiveProduct = await _productCollection.Find(x => x.BrandId == guidId && !x.IsDeleted && x.IsActive).AnyAsync();
            if (hasActiveProduct)
            {
                throw new Store.Catalog.Exceptions.BusinessException("Silmek istediğiniz Markaya bağlı Aktif bir ürün olduğundan bu kaydı Silemezsiniz!");
            }

            var update = Builders<Brand>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, DateTime.Now);

            await _brandCollection.UpdateOneAsync(x => x.Id == guidId, update);
            await _cacheService.RemoveByPrefixAsync(CachePrefix);
        }

        public async Task<List<ResultBrandDto>> GetAllBrandAsync(bool showDeleted = false)
        {
            var result = await _cacheService.GetOrSetAsync($"{CachePrefix}all:{showDeleted}", async () =>
            {
                var filter = showDeleted ? FilterDefinition<Brand>.Empty : Builders<Brand>.Filter.Eq(x => x.IsDeleted, false);
                var values = await _brandCollection.Find(filter).ToListAsync();
                return _mapper.Map<List<ResultBrandDto>>(values);
            }, CacheDuration);

            return result ?? new List<ResultBrandDto>();
        }

        public async Task<GetByIdBrandDto?> GetByIdBrandAsync(Guid id)
        {
            var guidId = id;
            return await _cacheService.GetOrSetAsync($"{CachePrefix}id:{id}", async () =>
            {
                var entity = await _brandCollection.Find<Brand>(x => x.Id == guidId && !x.IsDeleted).FirstOrDefaultAsync();
                return _mapper.Map<GetByIdBrandDto?>(entity);
            }, CacheDuration);
        }

        public async Task UpdateBrandAsync(UpdateBrandDto updateBrandDto)
        {
            var guidId = updateBrandDto.Id;
            var existing = await _brandCollection.Find<Brand>(x => x.Id == guidId).FirstOrDefaultAsync();
            if (existing == null)
            {
                return;
            }

            if (existing.IsActive && !updateBrandDto.IsActive)
            {
                var hasActiveProduct = await _productCollection.Find(x => x.BrandId == guidId && !x.IsDeleted && x.IsActive).AnyAsync();
                if (hasActiveProduct)
                {
                    throw new Store.Catalog.Exceptions.BusinessException("Pasife Almak istediğiniz Markaya bağlı Aktif bir ürün olduğundan bu kaydı Pasife Alamazsınız!");
                }
            }

            _mapper.Map(updateBrandDto, existing);
            existing.UpdateUserId = UserContextAccessor.GetClientId();
            existing.UpdateDate = DateTime.Now;

            await _brandCollection.ReplaceOneAsync(x => x.Id == existing.Id, existing);
            await _cacheService.RemoveByPrefixAsync(CachePrefix);
        }

        public async Task RestoreBrandAsync(Guid id)
        {
            var guidId = id;
            var update = Builders<Brand>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.UpdateUserId, UserContextAccessor.GetClientId())
                .Set(x => x.UpdateDate, DateTime.Now);

            await _brandCollection.UpdateOneAsync(x => x.Id == guidId, update);
            await _cacheService.RemoveByPrefixAsync(CachePrefix);
        }
    }
}

