using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace Store.WebUI.Services.Interfaces
{
    public interface IFavoriteApiService
    {
        [Get("/api/favorites/{userId}")]
        Task<List<string>> GetUserFavoritesAsync(string userId, [Header("Authorization")] string authorization);

        [Post("/api/favorites")]
        Task AddFavoriteAsync([Body] AddFavoriteDto model, [Header("Authorization")] string authorization);

        [Delete("/api/favorites")]
        Task RemoveFavoriteAsync([Query] string userId, [Query] string productId, [Header("Authorization")] string authorization);
    }

    public class AddFavoriteDto
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
    }
}
