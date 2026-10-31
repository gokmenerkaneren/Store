using Refit;
using Store.DtoLayer.CatalogDtos.BestSellerDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IBestSellerApiService
    {
        [Get("/api/bestsellers")]
        Task<List<ResultBestSellerDto>> GetAllAsync();

        [Post("/api/bestsellers")]
        Task<HttpResponseMessage> CreateAsync([Body] CreateBestSellerDto createDto);

        [Put("/api/bestsellers")]
        Task<HttpResponseMessage> UpdateAsync([Body] UpdateBestSellerDto updateDto);

        [Delete("/api/bestsellers/{id}")]
        Task<HttpResponseMessage> DeleteAsync(Guid id);

        [Get("/api/bestsellers/{id}")]
        Task<UpdateBestSellerDto> GetByIdAsync(Guid id);
    }
}
