using Store.Catalog.Dtos.BrandDtos;

namespace Store.Catalog.Services.BrandServices
{
    public interface IBrandService
    {
        Task<List<ResultBrandDto>> GetAllBrandAsync(bool showDeleted = false);
        Task CreateBrandAsync(CreateBrandDto createBrandDto);
        Task UpdateBrandAsync(UpdateBrandDto updateBrandDto);
        Task DeleteBrandAsync(Guid id);
        Task RestoreBrandAsync(Guid id);
        Task<GetByIdBrandDto?> GetByIdBrandAsync(Guid id);
    }
}

