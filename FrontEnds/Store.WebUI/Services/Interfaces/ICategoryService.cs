using Refit;
using Store.DtoLayer.CatalogDtos.CategoryDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface ICategoryService
    {
        [Get("/api/categories")]
        Task<List<ResultCategoryDto>> GetAllCategoriesAsync();

        [Get("/api/categories?showDeleted={showDeleted}")]
        Task<List<ResultCategoryDto>> GetAllCategoriesAsync(bool showDeleted);

        [Post("/api/categories/datatables?showDeleted={showDeleted}")]
        Task<Store.Shared.Dtos.DataTablesResponse<ResultCategoryDto>> GetCategoriesForDataTablesAsync([Body] Store.Shared.Dtos.DataTablesRequest request, bool showDeleted = false);

        [Post("/api/categories")]
        Task<HttpResponseMessage> CreateCategoryAsync([Body] CreateCategoryDto createCategoryDto);

        [Put("/api/categories")]
        Task<HttpResponseMessage> UpdateCategoryAsync([Body] UpdateCategoryDto updateCategoryDto);

        [Delete("/api/categories/{id}")]
        Task<HttpResponseMessage> DeleteCategoryAsync(Guid id);

        [Get("/api/categories/{id}")]
        Task<UpdateCategoryDto> GetCategoryByIdAsync(Guid id);

        [Post("/api/categories/Restore/{id}")]
        Task<HttpResponseMessage> RestoreCategoryAsync(Guid id);
    }
}
