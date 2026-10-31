using Store.Catalog.Dtos.ProductDtos;

namespace Store.Catalog.Services.ProductServices
{
    public interface IProductService
    {
        Task<List<ResultProductDto>> GetAllProductAsync(bool showDeleted = false);

        // DİKKAT: Dönüş tipi Task yerine Task<ResultProductDto> yapıldı!
        Task<ResultProductDto> CreateProductAsync(CreateProductDto createProductDto);

        Task UpdateProductAsync(UpdateProductDto updateProductDto);
        Task DeleteProductAsync(Guid id);
        Task RestoreProductAsync(Guid id);
        Task<GetByIdProductDto?> GetByIdProductAsync(Guid id);
        
        Task<Store.Shared.Dtos.DataTablesResponse<ResultProductDto>> GetDataTablesAsync(Store.Shared.Dtos.DataTablesRequest request, bool showDeleted = false);
    }
}

