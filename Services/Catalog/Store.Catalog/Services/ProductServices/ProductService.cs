using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using Store.Catalog.Dtos.ProductDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using Store.Shared.Enums;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Store.Catalog.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMapper _mapper;

        public ProductService(IDatabaseSettings _databaseSettings, IMapper mapper)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _mapper = mapper;
        }

        private FilterDefinition<Product>? BuildDuplicateFilter(string name, string code, string barcode, Guid? excludeId = null)
        {
            var filters = new List<FilterDefinition<Product>>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(name)}$", "i");
                filters.Add(Builders<Product>.Filter.Regex(x => x.Name, regex));
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(code)}$", "i");
                filters.Add(Builders<Product>.Filter.Regex(x => x.ProductCode, regex));
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(barcode)}$", "i");
                filters.Add(Builders<Product>.Filter.Regex(x => x.Barcode, regex));
            }

            if (filters.Count == 0)
            {
                return null;
            }

            var orFilter = Builders<Product>.Filter.Or(filters);
            if (excludeId.HasValue && excludeId.Value != Guid.Empty)
            {
                return Builders<Product>.Filter.And(Builders<Product>.Filter.Ne(x => x.Id, excludeId.Value), orFilter);
            }

            return orFilter;
        }

        public async Task<ResultProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var name = (createProductDto.Name ?? "").Trim();
            var code = (createProductDto.ProductCode ?? "").Trim();
            var barcode = (createProductDto.Barcode ?? "").Trim();

            var dupFilter = BuildDuplicateFilter(name, code, barcode);
            if (dupFilter != null)
            {
                var existing = await _productCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existing != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);
                    bool codeMatched = !string.IsNullOrWhiteSpace(code) && string.Equals((existing.ProductCode ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Ürün Adı" : (codeMatched ? "Ürün Kodu" : "Barkod");
                    string matchedValue = nameMatched ? existing.Name : (codeMatched ? existing.ProductCode : existing.Barcode);

                    string msg = Store.Catalog.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                    throw new InvalidOperationException(msg);
                }
            }

            var product = _mapper.Map<Product>(createProductDto);
            await _productCollection.InsertOneAsync(product);
            return _mapper.Map<ResultProductDto>(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var guidId = id;
            var update = Builders<Product>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, BaseEntity.GetLocalTime());

            await _productCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task RestoreProductAsync(Guid id)
        {
            var guidId = id;
            var update = Builders<Product>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.UpdateUserId, UserContextAccessor.GetClientId())
                .Set(x => x.UpdateDate, BaseEntity.GetLocalTime());

            await _productCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task<List<ResultProductDto>> GetAllProductAsync(bool showDeleted = false)
        {
            var filter = showDeleted ? FilterDefinition<Product>.Empty : Builders<Product>.Filter.Eq(x => x.IsDeleted, false);
            var values = await _productCollection.Find(filter).ToListAsync();
            return _mapper.Map<List<ResultProductDto>>(values);
        }

        public async Task<GetByIdProductDto?> GetByIdProductAsync(Guid id)
        {
            var guidId = id;
            var entity = await _productCollection.Find<Product>(x => x.Id == guidId && !x.IsDeleted).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdProductDto?>(entity);
        }

        public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var guidId = updateProductDto.Id;
            var name = (updateProductDto.Name ?? "").Trim();
            var code = (updateProductDto.ProductCode ?? "").Trim();
            var barcode = (updateProductDto.Barcode ?? "").Trim();

            var dupFilter = BuildDuplicateFilter(name, code, barcode, guidId);
            if (dupFilter != null)
            {
                var existing = await _productCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existing != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);
                    bool codeMatched = !string.IsNullOrWhiteSpace(code) && string.Equals((existing.ProductCode ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Ürün Adı" : (codeMatched ? "Ürün Kodu" : "Barkod");
                    string matchedValue = nameMatched ? existing.Name : (codeMatched ? existing.ProductCode : existing.Barcode);

                    string msg = Store.Catalog.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                    throw new InvalidOperationException(msg);
                }
            }

            var value = _mapper.Map<Product>(updateProductDto);
            value.Id = guidId;
            value.UpdateUserId = UserContextAccessor.GetClientId();
            value.UpdateDate = BaseEntity.GetLocalTime();

            await _productCollection.FindOneAndReplaceAsync(x => x.Id == guidId, value);
        }

        public async Task<Store.Shared.Dtos.DataTablesResponse<ResultProductDto>> GetDataTablesAsync(Store.Shared.Dtos.DataTablesRequest request, bool showDeleted = false)
        {
            var query = _productCollection.AsQueryable();

            if (!showDeleted)
            {
                query = query.Where(x => !x.IsDeleted);
            }

            var pagedQuery = Store.Shared.Extensions.DataTablesExtensions.ApplyDataTables(query, request, out int totalCount, out int filteredCount);

            // Execute the query
            var data = pagedQuery.ToList();

            var mappedData = _mapper.Map<List<ResultProductDto>>(data);

            return new Store.Shared.Dtos.DataTablesResponse<ResultProductDto>
            {
                Draw = request.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = filteredCount,
                Data = mappedData
            };
        }
    }
}

