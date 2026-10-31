using Store.Basket.Dtos;

namespace Store.Basket.Services
{
    public interface IBasketService
    {
        Task<BasketTotalDto> GetBasket(Guid UserId);
        Task SaveBasket(BasketTotalDto basketTotalDto);
        Task DeleteBasket(Guid UserId);

    }
}

