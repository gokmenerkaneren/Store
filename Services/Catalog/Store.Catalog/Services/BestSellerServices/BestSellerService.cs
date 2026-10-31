using AutoMapper;
using MongoDB.Driver;
using Store.Catalog.Dtos.BestSellerDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Catalog.Services.BestSellerServices
{
    public class BestSellerService : IBestSellerService
    {
        private readonly IMongoCollection<BestSeller> _bestSellerCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<ProductImage> _productImageCollection;
        private readonly IMapper _mapper;

        public BestSellerService(IMapper mapper, IDatabaseSettings _databaseSettings, IMongoDatabase database)
        {
            _bestSellerCollection = database.GetCollection<BestSeller>(_databaseSettings.BestSellerCollectionName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _productImageCollection = database.GetCollection<ProductImage>(_databaseSettings.ProductImageCollectionName);
            _mapper = mapper;
        }

        public async Task CreateBestSellerAsync(CreateBestSellerDto createBestSellerDto)
        {
            var existing = await _bestSellerCollection.Find(x => x.ProductId == createBestSellerDto.ProductId).FirstOrDefaultAsync();
            if (existing != null)
                return; // Mükerrer kayıt engelleme

            var value = _mapper.Map<BestSeller>(createBestSellerDto);
            await _bestSellerCollection.InsertOneAsync(value);
        }

        public async Task DeleteBestSellerAsync(Guid id)
        {
            await _bestSellerCollection.DeleteOneAsync(x => x.Id == id);
            
            // Reorder remaining items
            var remainingItems = await _bestSellerCollection.Find(x => true).SortBy(x => x.OrderIndex).ToListAsync();
            for (int i = 0; i < remainingItems.Count; i++)
            {
                if (remainingItems[i].OrderIndex != i + 1)
                {
                    remainingItems[i].OrderIndex = i + 1;
                    await _bestSellerCollection.ReplaceOneAsync(x => x.Id == remainingItems[i].Id, remainingItems[i]);
                }
            }
        }

        public async Task<List<ResultBestSellerDto>> GetAllBestSellerAsync()
        {
            var bestSellers = await _bestSellerCollection.Find(x => true).ToListAsync();
            var result = new List<ResultBestSellerDto>();

            // Detayları doldur
            foreach (var item in bestSellers)
            {
                var mapped = _mapper.Map<ResultBestSellerDto>(item);
                var product = await _productCollection.Find(x => x.Id == item.ProductId).FirstOrDefaultAsync();
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
        }

        public async Task<GetByIdBestSellerDto> GetByIdBestSellerAsync(Guid id)
        {
            var values = await _bestSellerCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdBestSellerDto>(values);
        }

        public async Task UpdateBestSellerAsync(UpdateBestSellerDto updateBestSellerDto)
        {
            var existing = await _bestSellerCollection.Find(x => x.Id == updateBestSellerDto.Id).FirstOrDefaultAsync();
            if(existing != null)
            {
                var values = _mapper.Map(updateBestSellerDto, existing);
                values.UpdateDate = BaseEntity.GetLocalTime();
                await _bestSellerCollection.ReplaceOneAsync(x => x.Id == updateBestSellerDto.Id, values);
            }
        }
    }
}
