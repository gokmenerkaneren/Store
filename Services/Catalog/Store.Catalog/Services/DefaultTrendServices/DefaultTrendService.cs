using AutoMapper;
using MongoDB.Driver;
using Store.Catalog.Dtos.DefaultTrendDtos;
using Store.Catalog.Entities;
using Store.Catalog.Services.CacheServices;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Catalog.Services.DefaultTrendServices
{
    public class DefaultTrendService : IDefaultTrendService
    {
        private readonly IMongoCollection<DefaultTrend> _defaultTrendCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<ProductImage> _productImageCollection;
        private readonly IMapper _mapper;
        private readonly ICatalogCacheService _cacheService;

        private const string CachePrefix = "catalog:trends:";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public DefaultTrendService(IMapper mapper, IDatabaseSettings _databaseSettings, IMongoDatabase database, ICatalogCacheService cacheService)
        {
            _defaultTrendCollection = database.GetCollection<DefaultTrend>(_databaseSettings.DefaultTrendCollectionName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _productImageCollection = database.GetCollection<ProductImage>(_databaseSettings.ProductImageCollectionName);
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task CreateDefaultTrendAsync(CreateDefaultTrendDto createDefaultTrendDto)
        {
            var existing = await _defaultTrendCollection.Find(x => x.ProductId == createDefaultTrendDto.ProductId).FirstOrDefaultAsync();
            if (existing != null)
                return; // Silently ignore duplicate

            var value = _mapper.Map<DefaultTrend>(createDefaultTrendDto);
            await _defaultTrendCollection.InsertOneAsync(value);
            await _cacheService.RemoveByPrefixAsync(CachePrefix);
        }

        public async Task DeleteDefaultTrendAsync(Guid id)
        {
            await _defaultTrendCollection.DeleteOneAsync(x => x.Id == id);

            try
            {
                // Reorder remaining items
                var remainingTrends = await _defaultTrendCollection.Find(x => true).SortBy(x => x.OrderIndex).ToListAsync();
                for (int i = 0; i < remainingTrends.Count; i++)
                {
                    if (remainingTrends[i].OrderIndex != i + 1)
                    {
                        remainingTrends[i].OrderIndex = i + 1;
                        await _defaultTrendCollection.ReplaceOneAsync(x => x.Id == remainingTrends[i].Id, remainingTrends[i]);
                    }
                }
            }
            finally
            {
                // Sıralama yarıda kalsa bile kayıt silindi; önbellek yine temizlenmeli.
                await _cacheService.RemoveByPrefixAsync(CachePrefix);
            }
        }

        public async Task<List<ResultDefaultTrendDto>> GetAllDefaultTrendAsync()
        {
            var cached = await _cacheService.GetOrSetAsync($"{CachePrefix}all", async () =>
            {
                var trends = await _defaultTrendCollection.Find(x => true).ToListAsync();
                var result = new List<ResultDefaultTrendDto>();

                foreach (var trend in trends)
                {
                    var mapped = _mapper.Map<ResultDefaultTrendDto>(trend);
                    var product = await _productCollection.Find(x => x.Id == trend.ProductId).FirstOrDefaultAsync();
                    if (product != null)
                    {
                        mapped.ProductName = product.Name;
                        mapped.ShortDescription = product.ShortDescription;

                        var image = await _productImageCollection.Find(x => x.ProductId == product.Id && x.IsMain).FirstOrDefaultAsync();
                        mapped.ImageUrl = image?.ImageUrl;
                    }
                    result.Add(mapped);
                }
                return result.OrderBy(x => x.OrderIndex).ToList();
            }, CacheDuration);

            return cached ?? new List<ResultDefaultTrendDto>();
        }

        public async Task<GetByIdDefaultTrendDto> GetByIdDefaultTrendAsync(Guid id)
        {
            var result = await _cacheService.GetOrSetAsync($"{CachePrefix}id:{id}", async () =>
            {
                var values = await _defaultTrendCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
                return _mapper.Map<GetByIdDefaultTrendDto>(values);
            }, CacheDuration);

            return result!;
        }

        public async Task UpdateDefaultTrendAsync(UpdateDefaultTrendDto updateDefaultTrendDto)
        {
            var existing = await _defaultTrendCollection.Find(x => x.Id == updateDefaultTrendDto.Id).FirstOrDefaultAsync();
            if(existing != null)
            {
                var values = _mapper.Map(updateDefaultTrendDto, existing);
                values.UpdateDate = BaseEntity.GetLocalTime();
                await _defaultTrendCollection.ReplaceOneAsync(x => x.Id == updateDefaultTrendDto.Id, values);
                await _cacheService.RemoveByPrefixAsync(CachePrefix);
            }
        }
    }
}