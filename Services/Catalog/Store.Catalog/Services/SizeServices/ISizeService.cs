using Store.Catalog.Dtos.SizeDtos;

namespace Store.Catalog.Services.SizeServices
{
    public interface ISizeService
    {
        Task<List<ResultSizeDto>> GetAllSizeAsync(bool showDeleted = false);
        Task CreateSizeAsync(CreateSizeDto createSizeDto);
        Task UpdateSizeAsync(UpdateSizeDto updateSizeDto);
        Task DeleteSizeAsync(Guid id);
        Task RestoreSizeAsync(Guid id);
        Task<GetByIdSizeDto?> GetByIdSizeAsync(Guid id);
    }
}
