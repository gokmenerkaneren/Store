using System;
using System.Threading.Tasks;
using Store.Catalog.Dtos.PricingDtos;

namespace Store.Catalog.Services.PricingServices
{
    public interface IPricingService
    {
        Task<CalculatedPriceDto> CalculatePriceAsync(Guid productId, Guid? dealerId = null);
    }
}
