using AutoMapper;
using MongoDB.Driver;
using Store.Catalog.Dtos.InstagramPostDtos;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Catalog.Services.InstagramPostServices
{
    public class InstagramPostService : IInstagramPostService
    {
        private readonly IMongoCollection<InstagramPost> _instagramPostCollection;
        private readonly IMapper _mapper;

        public InstagramPostService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _instagramPostCollection = database.GetCollection<InstagramPost>(_databaseSettings.InstagramPostCollectionName);
            _mapper = mapper;
        }

        public async Task CreateInstagramPostAsync(CreateInstagramPostDto createInstagramPostDto)
        {
            var value = _mapper.Map<InstagramPost>(createInstagramPostDto);
            await _instagramPostCollection.InsertOneAsync(value);
        }

        public async Task DeleteInstagramPostAsync(Guid id)
        {
            await _instagramPostCollection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<ResultInstagramPostDto>> GetActiveInstagramPostsAsync()
        {
            var values = await _instagramPostCollection.Find(x => x.IsActive).SortBy(x => x.OrderIndex).ToListAsync();
            return _mapper.Map<List<ResultInstagramPostDto>>(values);
        }

        public async Task<List<ResultInstagramPostDto>> GetAllInstagramPostsAsync()
        {
            var values = await _instagramPostCollection.Find(x => true).SortBy(x => x.OrderIndex).ToListAsync();
            return _mapper.Map<List<ResultInstagramPostDto>>(values);
        }

        public async Task<ResultInstagramPostDto> GetByIdInstagramPostAsync(Guid id)
        {
            var value = await _instagramPostCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<ResultInstagramPostDto>(value);
        }

        public async Task UpdateInstagramPostAsync(UpdateInstagramPostDto updateInstagramPostDto)
        {
            var value = _mapper.Map<InstagramPost>(updateInstagramPostDto);
            await _instagramPostCollection.FindOneAndReplaceAsync(x => x.Id == updateInstagramPostDto.Id, value);
        }
    }
}

