using Store.Catalog.Dtos.ColorDtos;

namespace Store.Catalog.Services.ColorServices
{
    public interface IColorService
    {
        Task<List<ResultColorDto>> GetAllColorAsync(bool showDeleted = false);
        Task CreateColorAsync(CreateColorDto createColorDto);
        Task UpdateColorAsync(UpdateColorDto updateColorDto);
        Task DeleteColorAsync(Guid id);
        Task RestoreColorAsync(Guid id);
        Task<GetByIdColorDto?> GetByIdColorAsync(Guid id);
    }
}
