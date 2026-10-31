using Microsoft.AspNetCore.Mvc;
using Store.Basket.Dtos;
using Store.Basket.LoginServices;
using Store.Basket.Services;

namespace Store.Basket.Controllers
{
    [Route("api/Baskets")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketService _basketServices;
        private readonly ILoginService _loginService;

        public BasketsController(IBasketService basketServices, ILoginService loginService)
        {
            _basketServices = basketServices;
            _loginService = loginService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyBasketDetail()
        {
            var user = User.Claims;
            var values = await _basketServices.GetBasket(_loginService.GetUserId);
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMyBasket([FromBody] BasketTotalDto basketTotalDto)
        {
            basketTotalDto.UserId = _loginService.GetUserId;
            await _basketServices.SaveBasket(basketTotalDto);
            return Ok(new { success = true, message = "Sepet kaydedildi" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMyBasket()
        {
            await _basketServices.DeleteBasket(_loginService.GetUserId);
            return Ok(new { success = true, message = "Sepet temizlendi" });
        }
    }
}