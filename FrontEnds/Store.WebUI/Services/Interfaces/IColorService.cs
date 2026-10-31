using Refit;
using Store.DtoLayer.CatalogDtos.ColorDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IColorService
    {
        [Get("/api/colors")]
        Task<List<ResultColorDto>> GetAllColorsAsync();

        [Get("/api/colors?showDeleted={showDeleted}")]
        Task<List<ResultColorDto>> GetAllColorsAsync(bool showDeleted);

        [Get("/api/colors/{id}")]
        Task<UpdateColorDto> GetColorByIdAsync(Guid id);

        [Post("/api/colors")]
        Task<HttpResponseMessage> CreateColorAsync([Body] CreateColorDto createColorDto);

        [Put("/api/colors")]
        Task<HttpResponseMessage> UpdateColorAsync([Body] UpdateColorDto updateColorDto);

        [Delete("/api/colors/{id}")]
        Task<HttpResponseMessage> DeleteColorAsync(Guid id);

        [Post("/api/colors/Restore/{id}")]
        Task<HttpResponseMessage> RestoreColorAsync(Guid id);
    }
}
