using System;
using System.Threading.Tasks;
using Store.Catalog.Dtos.ProductPriceDtos;

namespace Store.Catalog.Services.JobServices
{
    public interface IBulkPriceJobService
    {
        Task ExecuteBulkPriceUpdateAsync(BulkPriceUpdateDto request);
    }
}
