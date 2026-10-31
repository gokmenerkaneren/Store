using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using Store.Catalog.Dtos.MeasurementDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using Store.Shared.Enums;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Store.Catalog.Services.MeasurementServices
{
    public class MeasurementService : IMeasurementService
    {
        private readonly IMongoCollection<Measurement> _measurementCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMapper _mapper;

        public MeasurementService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _measurementCollection = database.GetCollection<Measurement>(_databaseSettings.MeasurementCollectionName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _mapper = mapper;
        }

        private FilterDefinition<Measurement>? BuildDuplicateFilter(string name, string code, string symbol, Guid? excludeId = null)
        {
            var filters = new List<FilterDefinition<Measurement>>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(name)}$", "i");
                filters.Add(Builders<Measurement>.Filter.Regex(x => x.Name, regex));
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(code)}$", "i");
                filters.Add(Builders<Measurement>.Filter.Regex(x => x.Code, regex));
            }

            if (!string.IsNullOrWhiteSpace(symbol))
            {
                var regex = new BsonRegularExpression($"^{Regex.Escape(symbol)}$", "i");
                filters.Add(Builders<Measurement>.Filter.Regex(x => x.Symbol, regex));
            }

            if (filters.Count == 0)
            {
                return null;
            }

            var orFilter = Builders<Measurement>.Filter.Or(filters);
            if (excludeId.HasValue && excludeId.Value != Guid.Empty)
            {
                return Builders<Measurement>.Filter.And(Builders<Measurement>.Filter.Ne(x => x.Id, excludeId.Value), orFilter);
            }

            return orFilter;
        }

        public async Task CreateMeasurementAsync(CreateMeasurementDto createMeasurementDto)
        {
            var name = (createMeasurementDto.Name ?? "").Trim();
            var code = (createMeasurementDto.Code ?? "").Trim();
            var symbol = (createMeasurementDto.Symbol ?? "").Trim();

            var dupFilter = BuildDuplicateFilter(name, code, symbol);
            if (dupFilter != null)
            {
                var existing = await _measurementCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existing != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);
                    bool codeMatched = !string.IsNullOrWhiteSpace(code) && string.Equals((existing.Code ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Ölçü Birimi Adı" : (codeMatched ? "Ölçü Birimi Kodu" : "Ölçü Birimi Sembolü");
                    string matchedValue = nameMatched ? existing.Name : (codeMatched ? existing.Code : existing.Symbol);

                    string msg = Store.Catalog.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                    throw new InvalidOperationException(msg);
                }
            }

            var entity = _mapper.Map<Measurement>(createMeasurementDto);
            await _measurementCollection.InsertOneAsync(entity);
        }

        public async Task DeleteMeasurementAsync(Guid id)
        {
            var guidId = id;
            var hasActiveProduct = await _productCollection.Find(x => x.MeasurementId == guidId && !x.IsDeleted && x.IsActive).AnyAsync();
            if (hasActiveProduct)
            {
                throw new Store.Catalog.Exceptions.BusinessException("Silmek istediğiniz Ölçü Birimine bağlı Aktif bir ürün olduğundan bu kaydı Silemezsiniz!");
            }

            var update = Builders<Measurement>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, BaseEntity.GetLocalTime());

            await _measurementCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task RestoreMeasurementAsync(Guid id)
        {
            var guidId = id;
            var update = Builders<Measurement>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.UpdateUserId, UserContextAccessor.GetClientId())
                .Set(x => x.UpdateDate, BaseEntity.GetLocalTime());

            await _measurementCollection.UpdateOneAsync(x => x.Id == guidId, update);
        }

        public async Task<List<ResultMeasurementDto>> GetAllMeasurementAsync(bool showDeleted = false)
        {
            var filter = showDeleted ? FilterDefinition<Measurement>.Empty : Builders<Measurement>.Filter.Eq(x => x.IsDeleted, false);
            var values = await _measurementCollection.Find(filter).ToListAsync();
            return _mapper.Map<List<ResultMeasurementDto>>(values);
        }

        public async Task<GetByIdMeasurementDto?> GetByIdMeasurementAsync(Guid id)
        {
            var guidId = id;
            var entity = await _measurementCollection.Find<Measurement>(x => x.Id == guidId && !x.IsDeleted).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdMeasurementDto?>(entity);
        }

        public async Task UpdateMeasurementAsync(UpdateMeasurementDto updateMeasurementDto)
        {
            var guidId = updateMeasurementDto.Id;
            var name = (updateMeasurementDto.Name ?? "").Trim();
            var code = (updateMeasurementDto.Code ?? "").Trim();
            var symbol = (updateMeasurementDto.Symbol ?? "").Trim();

            var currentEntity = await _measurementCollection.Find(m => m.Id == guidId).FirstOrDefaultAsync();
            if (currentEntity == null)
            {
                throw new InvalidOperationException("Güncellenecek kayıt bulunamadı.");
            }

            var dupFilter = BuildDuplicateFilter(name, code, symbol, guidId);
            if (dupFilter != null)
            {
                var existing = await _measurementCollection.Find(dupFilter).FirstOrDefaultAsync();
                if (existing != null)
                {
                    bool nameMatched = !string.IsNullOrWhiteSpace(name) && string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);
                    bool codeMatched = !string.IsNullOrWhiteSpace(code) && string.Equals((existing.Code ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase);

                    string fieldLabel = nameMatched ? "Ölçü Birimi Adı" : (codeMatched ? "Ölçü Birimi Kodu" : "Ölçü Birimi Sembolü");
                    string matchedValue = nameMatched ? existing.Name : (codeMatched ? existing.Code : existing.Symbol);

                    string msg = Store.Catalog.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                    throw new InvalidOperationException(msg);
                }
            }

            if (currentEntity.IsActive && !updateMeasurementDto.IsActive)
            {
                var hasActiveProduct = await _productCollection.Find(x => x.MeasurementId == guidId && !x.IsDeleted && x.IsActive).AnyAsync();
                if (hasActiveProduct)
                {
                    throw new Store.Catalog.Exceptions.BusinessException("Pasife Almak istediğiniz Ölçü Birimine bağlı Aktif bir ürün olduğundan bu kaydı Pasife Alamazsınız!");
                }
            }

            var value = _mapper.Map<Measurement>(updateMeasurementDto);
            value.Id = guidId;
            value.InsertUserId = currentEntity.InsertUserId;
            value.InsertDate = currentEntity.InsertDate;
            value.IsDeleted = currentEntity.IsDeleted;
            value.DeleteUserId = currentEntity.DeleteUserId;
            value.DeleteDate = currentEntity.DeleteDate;
            value.UpdateUserId = UserContextAccessor.GetClientId();
            value.UpdateDate = BaseEntity.GetLocalTime();

            await _measurementCollection.FindOneAndReplaceAsync(x => x.Id == guidId, value);
        }
    }
}

