using Refit;
using Store.DtoLayer.CatalogDtos.DefaultSlider1Dtos;
using Store.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IDefaultSlider1Service
    {
        [Get("/api/defaultslider1?showDeleted={showDeleted}")]
        Task<List<ResultDefaultSlider1Dto>> GetAllAsync(bool showDeleted = false);

        [Post("/api/defaultslider1/datatables?showDeleted={showDeleted}")]
        Task<DataTablesResponse<ResultDefaultSlider1Dto>> GetForDataTablesAsync([Body] DataTablesRequest request, bool showDeleted = false);

        [Post("/api/defaultslider1")]
        Task<HttpResponseMessage> CreateAsync([Body] CreateDefaultSlider1Dto createDto);

        [Put("/api/defaultslider1")]
        Task<HttpResponseMessage> UpdateAsync([Body] UpdateDefaultSlider1Dto updateDto);

        [Delete("/api/defaultslider1/{id}")]
        Task<HttpResponseMessage> DeleteAsync(Guid id);

        [Get("/api/defaultslider1/{id}")]
        Task<UpdateDefaultSlider1Dto> GetByIdAsync(Guid id);

        [Post("/api/defaultslider1/Restore/{id}")]
        Task<HttpResponseMessage> RestoreAsync(Guid id);
    }
}
