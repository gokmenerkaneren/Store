using AutoMapper;
using MongoDB.Driver;
using Store.Catalog.Dtos.NewProductDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Catalog.Services.NewProductServices
{
    public class NewProductService : INewProductService
    {
        private readonly IMongoCollection<NewProduct> _newProductCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<ProductImage> _productImageCollection;
        private readonly IMapper _mapper;

        public NewProductService(IMapper mapper, IDatabaseSettings _databaseSettings, IMongoDatabase database)
        {
            _newProductCollection = database.GetCollection<NewProduct>(_databaseSettings.NewProductCollectionName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _productImageCollection = database.GetCollection<ProductImage>(_databaseSettings.ProductImageCollectionName);
            _mapper = mapper;
        }

        public async Task CreateNewProductAsync(CreateNewProductDto createNewProductDto)
        {
            var existing = await _newProductCollection.Find(x => x.ProductId == createNewProductDto.ProductId).FirstOrDefaultAsync();
            if (existing != null)
                return; // Mükerrer kayıt engelleme

            var value = _mapper.Map<NewProduct>(createNewProductDto);
            await _newProductCollection.InsertOneAsync(value);
        }

        public async Task DeleteNewProductAsync(Guid id)
        {
            await _newProductCollection.DeleteOneAsync(x => x.Id == id);
            
            // Reorder remaining items
            var remainingItems = await _newProductCollection.Find(x => true).SortBy(x => x.OrderIndex).ToListAsync();
            for (int i = 0; i < remainingItems.Count; i++)
            {
                if (remainingItems[i].OrderIndex != i + 1)
                {
                    remainingItems[i].OrderIndex = i + 1;
                    await _newProductCollection.ReplaceOneAsync(x => x.Id == remainingItems[i].Id, remainingItems[i]);
                }
            }
        }

        public async Task<List<ResultNewProductDto>> GetAllNewProductAsync()
        {
            var newProducts = await _newProductCollection.Find(x => true).ToListAsync();
            var result = new List<ResultNewProductDto>();

            // Detayları doldur
            foreach (var item in newProducts)
            {
                var mapped = _mapper.Map<ResultNewProductDto>(item);
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

        public async Task<GetByIdNewProductDto> GetByIdNewProductAsync(Guid id)
        {
            var values = await _newProductCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdNewProductDto>(values);
        }

        public async Task UpdateNewProductAsync(UpdateNewProductDto updateNewProductDto)
        {
            var existing = await _newProductCollection.Find(x => x.Id == updateNewProductDto.Id).FirstOrDefaultAsync();
            if(existing != null)
            {
                var values = _mapper.Map(updateNewProductDto, existing);
                values.UpdateDate = BaseEntity.GetLocalTime();
                await _newProductCollection.ReplaceOneAsync(x => x.Id == updateNewProductDto.Id, values);
            }
        }
    }
}
