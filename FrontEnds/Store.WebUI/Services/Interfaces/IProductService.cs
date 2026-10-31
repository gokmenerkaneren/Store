using Refit;
using Store.DtoLayer.CatalogDtos.ProductDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IProductService
    {
        [Get("/api/products")]
        Task<List<ResultProductDto>> GetAllProductsAsync();

        [Post("/api/products")]
        Task<HttpResponseMessage> CreateProductAsync([Body] CreateProductDto createProductDto);

        [Put("/api/products")]
        Task<HttpResponseMessage> UpdateProductAsync([Body] UpdateProductDto updateProductDto);

        [Delete("/api/products/{id}")]
        Task<HttpResponseMessage> DeleteProductAsync(Guid id);

        [Get("/api/products/{id}")]
        Task<UpdateProductDto> GetProductByIdAsync(Guid id);
    }
}
