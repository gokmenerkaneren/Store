using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Store.Catalog.Dtos.CategoryDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using Store.Shared.Enums;
using System.Text.RegularExpressions;

namespace Store.Catalog.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IMapper mapper, IDatabaseSettings _databaseSettings, IMongoDatabase database, ILogger<CategoryService> logger)
        {        
            _categoryCollection = database.GetCollection<Category>(_databaseSettings.CategoryCollectionName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _mapper = mapper;
            _logger = logger;
        }

        private FilterDefinition<Category>? BuildDuplicateFilter(string name, string code, Guid? excludeId = null)
        {
            var filters = new List<FilterDefinition<Category>>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(name)}$", "i");
                filters.Add(Builders<Category>.Filter.Regex(x => x.Name, regex));
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(code)}$", "i");
                filters.Add(Builders<Category>.Filter.Regex(x => x.Code, regex));
            }

            if (filters.Count == 0)
            {
                return null;
            }

            var orFilter = Builders<Category>.Filter.Or(filters);
            if (excludeId.HasValue && excludeId.Value != Guid.Empty)
            {
                return Builders<Category>.Filter.And(Builders<Category>.Filter.Ne(x => x.Id, excludeId.Value), orFilter);
            }

            return orFilter;
        }

        public async Task<Store.Shared.Dtos.DataTablesResponse<ResultCategoryDto>> GetCategoriesForDataTablesAsync(Store.Shared.Dtos.DataTablesRequest request, bool showDeleted = false)
        {
            var query = _categoryCollection.AsQueryable();
            if (!showDeleted)
            {
                query = query.Where(x => x.IsDeleted == false);
            }

            int totalCount;
            int filteredCount;

            var filteredQuery = Store.Shared.Extensions.DataTablesExtensions.ApplyDataTables(query, request, out totalCount, out filteredCount);
            
            var data = await Task.FromResult(filteredQuery.ToList()); // Or use driver specific ToListAsync if cast is possible, but ToList() evaluates it synchronously which is fine for IQueryable over Mongo or just MongoQueryable.
            
            var mappedData = _mapper.Map<List<ResultCategoryDto>>(data);

            return new Store.Shared.Dtos.DataTablesResponse<ResultCategoryDto>
            {
                Draw = request.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = filteredCount,
                Data = mappedData
            };
        }

        private async Task<bool> HasActiveProductsAsync(Guid categoryId, List<Category> allCategories)
        {
            var categoryIdsToCheck = new HashSet<Guid> { categoryId };
            bool added;
            do
            {
                added = false;
                foreach (var cat in allCategories)
                {
                    if (cat.ParentId.HasValue && cat.ParentId.Value != Guid.Empty && categoryIdsToCheck.Contains(cat.ParentId.Value) && !categoryIdsToCheck.Contains(cat.Id))
                    {
                        categoryIdsToCheck.Add(cat.Id);
                        added = true;
                    }
                }
            } while (added);

            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(x => x.IsDeleted, false),
                Builders<Product>.Filter.Eq(x => x.IsActive, true),
                Builders<Product>.Filter.In(x => x.CategoryId, categoryIdsToCheck)
            );

            return await _productCollection.Find(filter).AnyAsync();
        }

        private static bool HasActiveSubcategories(Guid categoryId, List<Category> allCategories)
        {
            foreach (var category in allCategories.Where(x => x.IsActive && x.Id != categoryId))
            {
                var currentParentId = category.ParentId;
                while (currentParentId != null && currentParentId != Guid.Empty)
                {
                    if (currentParentId == categoryId)
                    {
                        return true;
                    }
                    var parent = allCategories.FirstOrDefault(x => x.Id == currentParentId);
                    currentParentId = parent?.ParentId;
                }
            }
            return false;
        }

        private static bool IsParentChainActive(Guid? parentId, List<Category> allCategories)
        {
            var currentParentId = parentId;
            while (currentParentId != null && currentParentId != Guid.Empty)
            {
                var parent = allCategories.FirstOrDefault(x => x.Id == currentParentId);
                if (parent == null || !parent.IsActive)
                {
                    return false;
                }
                currentParentId = parent.ParentId;
            }
            return true;
        }

        public async Task AddorEditAsync(CreateCategoryDto createCategoryDto)
        {
            var name = (createCategoryDto.Name ?? "").Trim();
            var code = (createCategoryDto.Code ?? "").Trim();

            var dupFilter = BuildDuplicateFilter(name, code);
            if (dupFilter != null)
            {
                var existing = await _categoryCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existing != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Kategori Adı" : "Kategori Kodu";
                    string matchedValue = nameMatched ? existing.Name : existing.Code;

                    string msg = Store.Catalog.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                    throw new Store.Catalog.Exceptions.BusinessException(msg);
                }
            }

            var value = _mapper.Map<Category>(createCategoryDto);
            await _categoryCollection.InsertOneAsync(value);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var guidId = id;
            var allCategories = await _categoryCollection.Find(x => !x.IsDeleted).ToListAsync();

            if (HasActiveSubcategories(guidId, allCategories))
            {
                throw new Store.Catalog.Exceptions.BusinessException("Aktif Alt Kategorisi Olduğu İçin Silemezsiniz !");
            }

            if (await HasActiveProductsAsync(guidId, allCategories))
            {
                throw new Store.Catalog.Exceptions.BusinessException("Silmek istediğiniz Kategoriye bağlı Aktif bir ürün olduğundan bu kaydı Silemezsiniz!");
            }

            var update = Builders<Category>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.IsActive, false)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, BaseEntity.GetLocalTime());

            await _categoryCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task RestoreCategoryAsync(Guid id)
        {
            var guidId = id;
            var categoryToRestore = await _categoryCollection.Find<Category>(x => x.Id == guidId).FirstOrDefaultAsync();
            if (categoryToRestore == null)
            {
                throw new Store.Catalog.Exceptions.BusinessException("Kategori bulunamadı.");
            }

            if (categoryToRestore.ParentId.HasValue && categoryToRestore.ParentId.Value != Guid.Empty)
            {
                var allCategories = await _categoryCollection.Find(x => !x.IsDeleted).ToListAsync();
                if (!IsParentChainActive(categoryToRestore.ParentId, allCategories))
                {
                    throw new Store.Catalog.Exceptions.BusinessException("Aktif Üst Kategorisi Olmadığı İçin Bu Kaydı Geri Alamazsınız !");
                }
            }

            var update = Builders<Category>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.IsActive, true)
                .Set(x => x.UpdateUserId, UserContextAccessor.GetClientId())
                .Set(x => x.UpdateDate, BaseEntity.GetLocalTime());

            await _categoryCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task<List<ResultCategoryDto>> GetAllCategoryAsync(bool showDeleted = false)
        {
            var filter = showDeleted ? FilterDefinition<Category>.Empty : Builders<Category>.Filter.Eq(x => x.IsDeleted, false);
            var values = await _categoryCollection.Find(filter).ToListAsync();
            return _mapper.Map<List<ResultCategoryDto>>(values);
        }

        public async Task<GetByIdCategoryDto?> GetByIdCategoryAsync(Guid id)
        {
            var guidId = id;
            var entity = await _categoryCollection.Find<Category>(x => x.Id == guidId && !x.IsDeleted).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdCategoryDto?>(entity);
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            var guidId = updateCategoryDto.Id;
            var name = (updateCategoryDto.Name ?? "").Trim();
            var code = (updateCategoryDto.Code ?? "").Trim();

            var dupFilter = BuildDuplicateFilter(name, code, guidId);
            if (dupFilter != null)
            {
                var existingDuplicate = await _categoryCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existingDuplicate != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existingDuplicate.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Kategori Adı" : "Kategori Kodu";
                    string matchedValue = nameMatched ? existingDuplicate.Name : existingDuplicate.Code;

                    string msg = Store.Catalog.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existingDuplicate.IsDeleted, existingDuplicate.IsActive);
                    throw new Store.Catalog.Exceptions.BusinessException(msg);
                }
            }

            var existing = await _categoryCollection.Find<Category>(x => x.Id == guidId).FirstOrDefaultAsync();
            if (existing == null)
            {
                return;
            }

            // 1) Aktiften Pasife çekme: Aktif alt kategorisi veya aktif ürünü varsa engellenecek
            if (existing.IsActive && !updateCategoryDto.IsActive)
            {
                var allCategories = await _categoryCollection.Find(x => !x.IsDeleted).ToListAsync();

                if (HasActiveSubcategories(guidId, allCategories))
                {
                    throw new Store.Catalog.Exceptions.BusinessException("Aktif Alt Kategori / Kategorileri Olduğu İçin bu Kaydı Pasife Alamazsınız !");
                }

                if (await HasActiveProductsAsync(guidId, allCategories))
                {
                    throw new Store.Catalog.Exceptions.BusinessException("Pasifge Almak istediğiniz Kategoriye bağlı Aktif bir ürün olduğundan bu kaydı Pasife Alamazsınız!");
                }
            }

            // 2) Pasiften Aktife alma: Üst kategorisi pasifse engellenecek
            if (!existing.IsActive && updateCategoryDto.IsActive)
            {
                var parentGuid = updateCategoryDto.ParentId ?? existing.ParentId;
                if (parentGuid.HasValue && parentGuid.Value != Guid.Empty)
                {
                    var allCategories = await _categoryCollection.Find(x => !x.IsDeleted).ToListAsync();
                    if (!IsParentChainActive(parentGuid, allCategories))
                    {
                        throw new Store.Catalog.Exceptions.BusinessException("Üst Kategorisi Pasif Olan bir Kayıt Aktif Edilemez !");
                    }
                }
            }

            _mapper.Map(updateCategoryDto, existing);

            existing.UpdateUserId = UserContextAccessor.GetClientId();
            existing.UpdateDate = BaseEntity.GetLocalTime();

            await _categoryCollection.ReplaceOneAsync(x => x.Id == existing.Id, existing);
        }
    }
}


