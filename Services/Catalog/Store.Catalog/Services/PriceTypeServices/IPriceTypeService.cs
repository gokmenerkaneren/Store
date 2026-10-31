using Store.Catalog.Dtos.PriceTypeDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Catalog.Services.PriceTypeServices
{
    public interface IPriceTypeService
    {
        Task<List<ResultPriceTypeDto>> GetAllPriceTypeAsync(bool showDeleted = false);
        Task CreatePriceTypeAsync(CreatePriceTypeDto createPriceTypeDto);
        Task UpdatePriceTypeAsync(UpdatePriceTypeDto updatePriceTypeDto);
        Task DeletePriceTypeAsync(Guid id);
        Task RestorePriceTypeAsync(Guid id);
        Task<GetByIdPriceTypeDto> GetByIdPriceTypeAsync(Guid id);
        Task<bool> CheckIfPriceTypeExistsAsync(string name, string code, string id = null);
    }
}

