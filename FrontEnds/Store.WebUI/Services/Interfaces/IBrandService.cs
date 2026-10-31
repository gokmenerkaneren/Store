using Refit;
using Store.DtoLayer.CatalogDtos.BrandDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IBrandService
    {
        [Get("/api/brands")]
        Task<List<ResultBrandDto>> GetAllBrandsAsync();

        [Get("/api/brands?showDeleted={showDeleted}")]
        Task<List<ResultBrandDto>> GetAllBrandsAsync(bool showDeleted);

        [Post("/api/brands")]
        Task<HttpResponseMessage> CreateBrandAsync([Body] CreateBrandDto createBrandDto);

        [Put("/api/brands")]
        Task<HttpResponseMessage> UpdateBrandAsync([Body] UpdateBrandDto updateBrandDto);

        [Delete("/api/brands/{id}")]
        Task<HttpResponseMessage> DeleteBrandAsync(Guid id);

        [Get("/api/brands/{id}")]
        Task<UpdateBrandDto> GetBrandByIdAsync(Guid id);

        [Post("/api/brands/Restore/{id}")]
        Task<HttpResponseMessage> RestoreBrandAsync(Guid id);
    }
}
