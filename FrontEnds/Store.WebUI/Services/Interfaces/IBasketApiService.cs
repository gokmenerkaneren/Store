using Refit;
using Store.DtoLayer.BasketDtos;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IBasketApiService
    {
        [Get("/api/Baskets")]
        Task<BasketTotalDto> GetMyBasketDetailAsync();

        [Post("/api/Baskets")]
        Task SaveMyBasketAsync([Body] BasketTotalDto basketTotalDto);

        [Delete("/api/Baskets")]
        Task DeleteMyBasketAsync();
    }
}
