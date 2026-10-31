using Store.Catalog.Dtos.NewProductDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Catalog.Services.NewProductServices
{
    public interface INewProductService
    {
        Task<List<ResultNewProductDto>> GetAllNewProductAsync();
        Task CreateNewProductAsync(CreateNewProductDto createNewProductDto);
        Task UpdateNewProductAsync(UpdateNewProductDto updateNewProductDto);
        Task DeleteNewProductAsync(Guid id);
        Task<GetByIdNewProductDto> GetByIdNewProductAsync(Guid id);
    }
}
