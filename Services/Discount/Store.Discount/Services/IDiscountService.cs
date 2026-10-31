using Store.Discount.Dtos;

namespace Store.Discount.Services
{
    public interface IDiscountService
    {
        Task<List<ResultCouponDto>> GetAllCouponsAsync();
        Task CreateCouponAsync(CreateCouponDto createCouponDto);
        Task UpdateCouponAsync(UpdateCouponDto updateCouponDto);
        Task DeleteCouponAsync(Guid id);
        Task<GetByIdCouponDto> GetByIdCouponAsync(Guid id);

    }
}
