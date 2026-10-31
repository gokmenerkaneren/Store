using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using Store.Catalog.Dtos.CurrencyDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using Store.Shared.Enums;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Store.Catalog.Services.CurrencyServices
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IMongoCollection<Currency> _currencyCollection;
        private readonly IMongoCollection<ProductPrice> _productPriceCollection;
        private readonly IMapper _mapper;

        public CurrencyService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _currencyCollection = database.GetCollection<Currency>(_databaseSettings.CurrencyCollectionName);
            _productPriceCollection = database.GetCollection<ProductPrice>(_databaseSettings.ProductPriceCollectionName);
            _mapper = mapper;
        }

        private FilterDefinition<Currency>? BuildDuplicateFilter(string name, string code, string symbol, Guid? excludeId = null)
        {
            var filters = new List<FilterDefinition<Currency>>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(name)}$", "i");
                filters.Add(Builders<Currency>.Filter.Regex(x => x.Name, regex));
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(code)}$", "i");
                filters.Add(Builders<Currency>.Filter.Regex(x => x.Code, regex));
            }

            if (!string.IsNullOrWhiteSpace(symbol))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(symbol)}$", "i");
                filters.Add(Builders<Currency>.Filter.Regex(x => x.Symbol, regex));
            }

            if (filters.Count == 0)
            {
                return null;
            }

            var orFilter = Builders<Currency>.Filter.Or(filters);
            if (excludeId.HasValue && excludeId.Value != Guid.Empty)
            {
                return Builders<Currency>.Filter.And(Builders<Currency>.Filter.Ne(x => x.Id, excludeId.Value), orFilter);
            }

            return orFilter;
        }

        public async Task CreateCurrencyAsync(CreateCurrencyDto createCurrencyDto)
        {
            var name = (createCurrencyDto.Name ?? "").Trim();
            var code = (createCurrencyDto.Code ?? "").Trim();
            var symbol = (createCurrencyDto.Symbol ?? "").Trim();

            var dupFilter = BuildDuplicateFilter(name, code, symbol);
            if (dupFilter != null)
            {
                var existing = await _currencyCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existing != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);
                    bool codeMatched = !string.IsNullOrWhiteSpace(code) && string.Equals((existing.Code ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Para Birimi Adı" : (codeMatched ? "Para Birimi Kodu" : "Para Birimi Sembolü");
                    string matchedValue = nameMatched ? existing.Name : (codeMatched ? existing.Code : existing.Symbol);

                    string msg = Store.Catalog.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                    throw new InvalidOperationException(msg);
                }
            }

            var entity = _mapper.Map<Currency>(createCurrencyDto);
            await _currencyCollection.InsertOneAsync(entity);
        }

        public async Task DeleteCurrencyAsync(Guid id)
        {
            var guidId = id;
            var hasActivePrice = await _productPriceCollection.Find(x => x.CurrencyId == guidId && !x.IsDeleted && x.IsActive).AnyAsync();
            if (hasActivePrice)
            {
                throw new Store.Catalog.Exceptions.BusinessException("Silmek istediğiniz Para Birimine bağlı Aktif bir ürün fiyatı olduğundan bu kaydı Silemezsiniz!");
            }

            var update = Builders<Currency>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, BaseEntity.GetLocalTime());

            await _currencyCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task RestoreCurrencyAsync(Guid id)
        {
            var guidId = id;
            var update = Builders<Currency>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.UpdateUserId, UserContextAccessor.GetClientId())
                .Set(x => x.UpdateDate, BaseEntity.GetLocalTime());

            await _currencyCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task<List<ResultCurrencyDto>> GetAllCurrencyAsync(bool showDeleted = false)
        {
            var filter = showDeleted ? FilterDefinition<Currency>.Empty : Builders<Currency>.Filter.Eq(x => x.IsDeleted, false);
            var values = await _currencyCollection.Find(filter).ToListAsync();
            return _mapper.Map<List<ResultCurrencyDto>>(values);
        }

        public async Task<GetByIdCurrencyDto?> GetByIdCurrencyAsync(Guid id)
        {
            var guidId = id;
            var entity = await _currencyCollection.Find<Currency>(x => x.Id == guidId && !x.IsDeleted).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdCurrencyDto?>(entity);
        }

        public async Task UpdateCurrencyAsync(UpdateCurrencyDto updateCurrencyDto)
        {
            var guidId = updateCurrencyDto.Id;
            var name = (updateCurrencyDto.Name ?? "").Trim();
            var code = (updateCurrencyDto.Code ?? "").Trim();
            var symbol = (updateCurrencyDto.Symbol ?? "").Trim();

            var currentEntity = await _currencyCollection.Find(c => c.Id == guidId).FirstOrDefaultAsync();
            if (currentEntity == null)
            {
                throw new InvalidOperationException("Güncellenecek kayıt bulunamadı.");
            }

            var dupFilter = BuildDuplicateFilter(name, code, symbol, guidId);
            if (dupFilter != null)
            {
                var existing = await _currencyCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existing != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);
                    bool codeMatched = !string.IsNullOrWhiteSpace(code) && string.Equals((existing.Code ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Para Birimi Adı" : (codeMatched ? "Para Birimi Kodu" : "Para Birimi Sembolü");
                    string matchedValue = nameMatched ? existing.Name : (codeMatched ? existing.Code : existing.Symbol);

                    string msg = Store.Catalog.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                    throw new InvalidOperationException(msg);
                }
            }

            if (currentEntity.IsActive && !updateCurrencyDto.IsActive)
            {
                var hasActivePrice = await _productPriceCollection.Find(x => x.CurrencyId == guidId && !x.IsDeleted && x.IsActive).AnyAsync();
                if (hasActivePrice)
                {
                    throw new Store.Catalog.Exceptions.BusinessException("Pasife Almak istediğiniz Para Birimine bağlı Aktif bir ürün fiyatı olduğundan bu kaydı Pasife Alamazsınız!");
                }
            }

            var value = _mapper.Map<Currency>(updateCurrencyDto);
            value.Id = guidId;
            value.InsertUserId = currentEntity.InsertUserId;
            value.InsertDate = currentEntity.InsertDate;
            value.IsDeleted = currentEntity.IsDeleted;
            value.DeleteUserId = currentEntity.DeleteUserId;
            value.DeleteDate = currentEntity.DeleteDate;
            value.UpdateUserId = UserContextAccessor.GetClientId();
            value.UpdateDate = BaseEntity.GetLocalTime();

            await _currencyCollection.FindOneAndReplaceAsync(x => x.Id == guidId, value);
        }
    }
}

