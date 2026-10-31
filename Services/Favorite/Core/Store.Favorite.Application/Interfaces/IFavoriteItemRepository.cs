using Store.Favorite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Favorite.Application.Interfaces
{
    public interface IFavoriteItemRepository
    {
        Task<List<string>> GetUserFavoritesAsync(Guid userId);
        Task<bool> ExistsAsync(Guid userId, string productId);
        Task AddAsync(FavoriteItem favoriteItem);
        Task UpdateAsync(FavoriteItem favoriteItem);
        Task<FavoriteItem?> GetByUserAndProductAsync(Guid userId, string productId);
        Task RemoveAsync(FavoriteItem favoriteItem);
    }
}
