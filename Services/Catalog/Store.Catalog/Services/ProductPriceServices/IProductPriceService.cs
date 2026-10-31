using Store.Catalog.Dtos.ProductPriceDtos;

namespace Store.Catalog.Services.ProductPriceServices
{
    public interface IProductPriceService
    {
        Task<List<ResultProductPriceDto>> GetAllProductPriceAsync(bool showDeleted = false);
        Task CreateProductPriceAsync(CreateProductPriceDto createProductPriceDto);
        Task UpdateProductPriceAsync(UpdateProductPriceDto updateProductPriceDto);
        Task DeleteProductPriceAsync(Guid id);
        Task<GetByIdProductPriceDto?> GetByIdProductPriceAsync(Guid id);
        Task<List<ResultProductPriceDto>> GetByProductIdProductPriceAsync(Guid productId, bool showDeleted = false);
        Task RestoreProductPriceAsync(Guid id);
    }
}

