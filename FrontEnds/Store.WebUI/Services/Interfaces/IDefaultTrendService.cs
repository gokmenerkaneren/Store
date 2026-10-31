using Refit;
using Store.DtoLayer.CatalogDtos.DefaultTrendDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IDefaultTrendService
    {
        [Get("/api/defaulttrends")]
        Task<List<ResultDefaultTrendDto>> GetAllAsync();

        [Post("/api/defaulttrends")]
        Task<HttpResponseMessage> CreateAsync([Body] CreateDefaultTrendDto createDto);

        [Put("/api/defaulttrends")]
        Task<HttpResponseMessage> UpdateAsync([Body] UpdateDefaultTrendDto updateDto);

        [Delete("/api/defaulttrends/{id}")]
        Task<HttpResponseMessage> DeleteAsync(Guid id);

        [Get("/api/defaulttrends/{id}")]
        Task<UpdateDefaultTrendDto> GetByIdAsync(Guid id);
    }
}