using Refit;
using Store.DtoLayer.CatalogDtos.InstagramPostDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IInstagramPostService
    {
        [Get("/api/InstagramPosts")]
        Task<List<ResultInstagramPostDto>> GetAllInstagramPostsAsync();

        [Get("/api/InstagramPosts/GetActiveInstagramPosts")]
        Task<List<ResultInstagramPostDto>> GetActiveInstagramPostsAsync();

        [Get("/api/InstagramPosts/{id}")]
        Task<UpdateInstagramPostDto> GetInstagramPostByIdAsync(Guid id);

        [Post("/api/InstagramPosts")]
        Task CreateInstagramPostAsync([Body] CreateInstagramPostDto createInstagramPostDto);

        [Put("/api/InstagramPosts")]
        Task UpdateInstagramPostAsync([Body] UpdateInstagramPostDto updateInstagramPostDto);

        [Delete("/api/InstagramPosts/{id}")]
        Task DeleteInstagramPostAsync(Guid id);
    }
}

