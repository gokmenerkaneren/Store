using Refit;
using Store.DtoLayer.CatalogDtos.NewProductDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface INewProductApiService
    {
        [Get("/api/newproducts")]
        Task<List<ResultNewProductDto>> GetAllAsync();

        [Post("/api/newproducts")]
        Task<HttpResponseMessage> CreateAsync([Body] CreateNewProductDto createDto);

        [Put("/api/newproducts")]
        Task<HttpResponseMessage> UpdateAsync([Body] UpdateNewProductDto updateDto);

        [Delete("/api/newproducts/{id}")]
        Task<HttpResponseMessage> DeleteAsync(Guid id);

        [Get("/api/newproducts/{id}")]
        Task<UpdateNewProductDto> GetByIdAsync(Guid id);
    }
}
