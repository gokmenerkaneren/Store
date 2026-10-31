using Refit;
using Store.DtoLayer.CatalogDtos.SizeDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface ISizeService
    {
        [Get("/api/sizes")]
        Task<List<ResultSizeDto>> GetAllSizesAsync();

        [Get("/api/sizes?showDeleted={showDeleted}")]
        Task<List<ResultSizeDto>> GetAllSizesAsync(bool showDeleted);

        [Get("/api/sizes/{id}")]
        Task<UpdateSizeDto> GetSizeByIdAsync(Guid id);

        [Post("/api/sizes")]
        Task<HttpResponseMessage> CreateSizeAsync([Body] CreateSizeDto createSizeDto);

        [Put("/api/sizes")]
        Task<HttpResponseMessage> UpdateSizeAsync([Body] UpdateSizeDto updateSizeDto);

        [Delete("/api/sizes/{id}")]
        Task<HttpResponseMessage> DeleteSizeAsync(Guid id);

        [Post("/api/sizes/Restore/{id}")]
        Task<HttpResponseMessage> RestoreSizeAsync(Guid id);
    }
}
