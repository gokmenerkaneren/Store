using Store.Catalog.Dtos.InstagramPostDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Catalog.Services.InstagramPostServices
{
    public interface IInstagramPostService
    {
        Task<List<ResultInstagramPostDto>> GetAllInstagramPostsAsync();
        Task<List<ResultInstagramPostDto>> GetActiveInstagramPostsAsync();
        Task<ResultInstagramPostDto> GetByIdInstagramPostAsync(Guid id);
        Task CreateInstagramPostAsync(CreateInstagramPostDto createInstagramPostDto);
        Task UpdateInstagramPostAsync(UpdateInstagramPostDto updateInstagramPostDto);
        Task DeleteInstagramPostAsync(Guid id);
    }
}

