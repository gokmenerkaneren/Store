using Refit;
using Store.DtoLayer.B2BDtos;
using System;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IB2BApiService
    {
        [Get("/api/DealerFinancials/GetByDealerId/{dealerId}")]
        Task<ResultDealerFinancialDto?> GetDealerFinancialByDealerIdAsync(Guid dealerId);
    }
}
