using Store.Catalog.Dtos.CategoryDtos;

namespace Store.Catalog.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task <List<ResultCategoryDto>> GetAllCategoryAsync(bool showDeleted = false);
        Task<Store.Shared.Dtos.DataTablesResponse<ResultCategoryDto>> GetCategoriesForDataTablesAsync(Store.Shared.Dtos.DataTablesRequest request, bool showDeleted = false);
        Task AddorEditAsync(CreateCategoryDto createCategoryDto);
        Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
        Task DeleteCategoryAsync(Guid id);
        Task RestoreCategoryAsync(Guid id);
        Task<GetByIdCategoryDto?> GetByIdCategoryAsync(Guid id);
    }
}

