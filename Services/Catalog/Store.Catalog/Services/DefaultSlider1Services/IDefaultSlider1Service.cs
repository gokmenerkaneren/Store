using Store.Catalog.Dtos.DefaultSlider1Dtos;

namespace Store.Catalog.Services.DefaultSlider1Services
{
    public interface IDefaultSlider1Service
    {
        Task<List<ResultDefaultSlider1Dto>> GetAllAsync(bool showDeleted = false);
        Task<GetByIdDefaultSlider1Dto?> GetByIdAsync(Guid id);
        Task<Store.Shared.Dtos.DataTablesResponse<ResultDefaultSlider1Dto>> GetForDataTablesAsync(Store.Shared.Dtos.DataTablesRequest request, bool showDeleted = false);
        Task CreateAsync(CreateDefaultSlider1Dto dto);
        Task UpdateAsync(UpdateDefaultSlider1Dto dto);
        Task DeleteAsync(Guid id);
        Task RestoreAsync(Guid id);
    }
}
