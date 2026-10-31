using AutoMapper;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Store.Catalog.Dtos.DefaultSlider1Dtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;

namespace Store.Catalog.Services.DefaultSlider1Services
{
    public class DefaultSlider1Service : IDefaultSlider1Service
    {
        private readonly IMongoCollection<DefaultSlider1> _collection;
        private readonly IMapper _mapper;
        private readonly ILogger<DefaultSlider1Service> _logger;

        public DefaultSlider1Service(IMapper mapper, IDatabaseSettings databaseSettings, IMongoDatabase database, ILogger<DefaultSlider1Service> logger)
        {
            _collection = database.GetCollection<DefaultSlider1>(databaseSettings.DefaultSlider1CollectionName);
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ResultDefaultSlider1Dto>> GetAllAsync(bool showDeleted = false)
        {
            var filter = showDeleted
                ? FilterDefinition<DefaultSlider1>.Empty
                : Builders<DefaultSlider1>.Filter.Eq(x => x.IsDeleted, false);

            var values = await _collection.Find(filter)
                .SortBy(x => x.Sirano)
                .ToListAsync();

            return _mapper.Map<List<ResultDefaultSlider1Dto>>(values);
        }

        public async Task<GetByIdDefaultSlider1Dto?> GetByIdAsync(Guid id)
        {
            var entity = await _collection.Find(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdDefaultSlider1Dto?>(entity);
        }

        public async Task<Store.Shared.Dtos.DataTablesResponse<ResultDefaultSlider1Dto>> GetForDataTablesAsync(Store.Shared.Dtos.DataTablesRequest request, bool showDeleted = false)
        {
            var query = _collection.AsQueryable();
            if (!showDeleted)
                query = query.Where(x => x.IsDeleted == false);

            var filteredQuery = Store.Shared.Extensions.DataTablesExtensions.ApplyDataTables(query, request, out int totalCount, out int filteredCount);
            var data = await System.Threading.Tasks.Task.FromResult(filteredQuery.ToList());
            var mappedData = _mapper.Map<List<ResultDefaultSlider1Dto>>(data);

            return new Store.Shared.Dtos.DataTablesResponse<ResultDefaultSlider1Dto>
            {
                Draw = request.Draw,
                RecordsTotal = totalCount,
                RecordsFiltered = filteredCount,
                Data = mappedData
            };
        }

        public async Task CreateAsync(CreateDefaultSlider1Dto dto)
        {
            var entity = _mapper.Map<DefaultSlider1>(dto);
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(UpdateDefaultSlider1Dto dto)
        {
            var existing = await _collection.Find(x => x.Id == dto.Id).FirstOrDefaultAsync();
            if (existing == null) return;

            _mapper.Map(dto, existing);
            existing.UpdateUserId = UserContextAccessor.GetClientId();
            existing.UpdateDate = BaseEntity.GetLocalTime();

            await _collection.ReplaceOneAsync(x => x.Id == existing.Id, existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var update = Builders<DefaultSlider1>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.IsActive, false)
                .Set(x => x.DeleteUserId, UserContextAccessor.GetClientId())
                .Set(x => x.DeleteDate, BaseEntity.GetLocalTime());

            await _collection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task RestoreAsync(Guid id)
        {
            var update = Builders<DefaultSlider1>.Update
                .Set(x => x.IsDeleted, false)
                .Set(x => x.IsActive, true)
                .Set(x => x.UpdateUserId, UserContextAccessor.GetClientId())
                .Set(x => x.UpdateDate, BaseEntity.GetLocalTime());

            await _collection.UpdateOneAsync(x => x.Id == id, update);
        }
    }
}
