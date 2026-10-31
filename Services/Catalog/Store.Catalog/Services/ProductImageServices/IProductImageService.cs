using Store.Catalog.Dtos.ProductImageDtos;

namespace Store.Catalog.Services.ProductImageServices
{
    public interface IProductImageService
    {
        Task<List<ResultProductImageDto>> GetAllProductImageAsync(bool showDeleted = false);
        Task CreateProductImageAsync(CreateProductImageDto createProductImageDto);
        Task UpdateProductImageAsync(UpdateProductImageDto updateProductImageDto);
        Task DeleteProductImageAsync(Guid id);
        Task<GetByIdProductImageDto?> GetByIdProductImageAsync(Guid id);
        Task<List<ResultProductImageDto>> GetByProductIdProductImageAsync(Guid productId);
    }
}

