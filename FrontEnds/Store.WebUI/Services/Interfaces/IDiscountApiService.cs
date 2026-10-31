using Refit;
using Store.DtoLayer.DiscountDtos;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IDiscountApiService
    {
        [Get("/api/Discounts/GetByCode/{code}")]
        Task<ResultCouponDto?> GetDiscountCouponByCodeAsync(string code);
    }
}
