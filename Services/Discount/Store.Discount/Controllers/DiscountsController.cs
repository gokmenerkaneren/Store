using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Discount.Dtos;
using Store.Discount.Services;

namespace Store.Discount.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            var values = await _discountService.GetAllCouponsAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountCouponById(Guid id)
        {
            var values = await _discountService.GetByIdCouponAsync(id);
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("GetByCode/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var coupon = await _discountService.GetByCodeCouponAsync(code);
            if (coupon == null)
            {
                return NotFound(new { message = "Kupon bulunamadı." });
            }

            if (coupon.ValidDate < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Kuponun geçerlilik süresi dolmuş." });
            }

            return Ok(coupon);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CreateCouponDto createCouponDto)
        {
            await _discountService.CreateCouponAsync(createCouponDto);
            return Ok("Coupon created successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateCoupon(UpdateCouponDto updateCouponDto)
        {            
            await _discountService.UpdateCouponAsync(updateCouponDto);
            return Ok("Coupon updated successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscountCoupon(Guid id)
        {
            await _discountService.DeleteCouponAsync(id);
            return Ok("Coupon deleted successfully.");
        }       

            
    }
}
