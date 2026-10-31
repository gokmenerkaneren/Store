using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Catalog.Dtos.PriceTypeDtos;
using Store.Shared.Entities;
using Store.Catalog.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace Store.Catalog.Services.PriceTypeServices
{
    public class PriceTypeService : IPriceTypeService
    {
        private readonly IMongoCollection<PriceType> _priceTypeCollection;
        private readonly IMongoCollection<ProductPrice> _productPriceCollection;
        private readonly IMapper _mapper;

        public PriceTypeService(IMapper mapper, IDatabaseSettings databaseSettings, IMongoDatabase database)
        {
            _priceTypeCollection = database.GetCollection<PriceType>(databaseSettings.PriceTypeCollectionName);
            _productPriceCollection = database.GetCollection<ProductPrice>(databaseSettings.ProductPriceCollectionName);
            _mapper = mapper;
        }

        private FilterDefinition<PriceType>? BuildDuplicateFilter(string name, string code, Guid? excludeId = null)
        {
            var filters = new List<FilterDefinition<PriceType>>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(name)}$", "i");
                filters.Add(Builders<PriceType>.Filter.Regex(x => x.Name, regex));
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(code)}$", "i");
                filters.Add(Builders<PriceType>.Filter.Regex(x => x.Code, regex));
            }

            if (filters.Count == 0)
            {
                return null;
            }

            var orFilter = Builders<PriceType>.Filter.Or(filters);
            if (excludeId.HasValue && excludeId.Value != Guid.Empty)
            {
                return Builders<PriceType>.Filter.And(Builders<PriceType>.Filter.Ne(x => x.Id, excludeId.Value), orFilter);
            }

            return orFilter;
        }

        public async Task CreatePriceTypeAsync(CreatePriceTypeDto createPriceTypeDto)
        {
            var name = (createPriceTypeDto.Name ?? "").Trim();
            var code = (createPriceTypeDto.Code ?? "").Trim();

            var dupFilter = BuildDuplicateFilter(name, code);
            if (dupFilter != null)
            {
                var existing = await _priceTypeCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existing != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);
                    bool codeMatched = !string.IsNullOrWhiteSpace(code) && string.Equals((existing.Code ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Fiyat Tipi Adı" : "Fiyat Tipi Kodu";
                    string matchedValue = nameMatched ? existing.Name : existing.Code;

                    string msg = DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                    throw new InvalidOperationException(msg);
                }
            }

            var value = _mapper.Map<PriceType>(createPriceTypeDto);
            value.InsertUserId = UserContextAccessor.GetClientId();
            value.InsertDate = BaseEntity.GetLocalTime();

            await _priceTypeCollection.InsertOneAsync(value);
        }

        public async Task DeletePriceTypeAsync(Guid id)
        {
            var hasActivePrice = await _productPriceCollection.Find(x => x.PriceTypeId == id && !x.IsDeleted && x.IsActive).AnyAsync();
            if (hasActivePrice)
            {
                throw new Store.Catalog.Exceptions.BusinessException("Silmek istediğiniz Fiyat Tipine bağlı Aktif bir ürün fiyatı olduğundan bu kaydı Silemezsiniz!");
            }

            var update = Builders<PriceType>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.IsActive, false)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, BaseEntity.GetLocalTime());

            await _priceTypeCollection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task RestorePriceTypeAsync(Guid id)
        {
            var update = Builders<PriceType>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.IsActive, true)
                .Set(x => x.UpdateUserId, UserContextAccessor.GetClientId())
                .Set(x => x.UpdateDate, BaseEntity.GetLocalTime());

            await _priceTypeCollection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task<List<ResultPriceTypeDto>> GetAllPriceTypeAsync(bool showDeleted = false)
        {
            var filter = showDeleted ? FilterDefinition<PriceType>.Empty : Builders<PriceType>.Filter.Eq(x => x.IsDeleted, false);
            var values = await _priceTypeCollection.Find(filter).ToListAsync();
            return _mapper.Map<List<ResultPriceTypeDto>>(values);
        }

        public async Task<GetByIdPriceTypeDto> GetByIdPriceTypeAsync(Guid id)
        {
            var value = await _priceTypeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdPriceTypeDto>(value);
        }

        public async Task UpdatePriceTypeAsync(UpdatePriceTypeDto updatePriceTypeDto)
        {
            var guidId = updatePriceTypeDto.Id;
            var name = (updatePriceTypeDto.Name ?? "").Trim();
            var code = (updatePriceTypeDto.Code ?? "").Trim();

            var currentEntity = await _priceTypeCollection.Find(c => c.Id == guidId).FirstOrDefaultAsync();
            if (currentEntity == null)
            {
                throw new InvalidOperationException("Güncellenecek kayıt bulunamadı.");
            }

            var dupFilter = BuildDuplicateFilter(name, code, guidId);
            if (dupFilter != null)
            {
                var existing = await _priceTypeCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existing != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);
                    bool codeMatched = !string.IsNullOrWhiteSpace(code) && string.Equals((existing.Code ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Fiyat Tipi Adı" : "Fiyat Tipi Kodu";
                    string matchedValue = nameMatched ? existing.Name : existing.Code;

                    string msg = DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                    throw new InvalidOperationException(msg);
                }
            }

            if (currentEntity.IsActive && !updatePriceTypeDto.IsActive)
            {
                var hasActivePrice = await _productPriceCollection.Find(x => x.PriceTypeId == guidId && !x.IsDeleted && x.IsActive).AnyAsync();
                if (hasActivePrice)
                {
                    throw new Store.Catalog.Exceptions.BusinessException("Pasife Almak istediğiniz Fiyat Tipine bağlı Aktif bir ürün fiyatı olduğundan bu kaydı Pasife Alamazsınız!");
                }
            }

            var value = _mapper.Map<PriceType>(updatePriceTypeDto);
            value.Id = guidId;
            value.InsertUserId = currentEntity.InsertUserId;
            value.InsertDate = currentEntity.InsertDate;
            value.IsDeleted = currentEntity.IsDeleted;
            value.DeleteUserId = currentEntity.DeleteUserId;
            value.DeleteDate = currentEntity.DeleteDate;
            value.UpdateUserId = UserContextAccessor.GetClientId();
            value.UpdateDate = BaseEntity.GetLocalTime();

            await _priceTypeCollection.ReplaceOneAsync(x => x.Id == guidId, value);
        }

        public async Task<bool> CheckIfPriceTypeExistsAsync(string name, string code, string id = null)
        {
            return false;
        }
    }
}
