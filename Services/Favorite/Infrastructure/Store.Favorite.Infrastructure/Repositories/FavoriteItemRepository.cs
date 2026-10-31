using Microsoft.EntityFrameworkCore;
using Store.Favorite.Application.Interfaces;
using Store.Favorite.Domain.Entities;
using Store.Favorite.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Favorite.Infrastructure.Repositories
{
    public class FavoriteItemRepository : IFavoriteItemRepository
    {
        private readonly StoreFavoriteContext _context;

        public FavoriteItemRepository(StoreFavoriteContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FavoriteItem favoriteItem)
        {
            _context.FavoriteItems.Add(favoriteItem);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid userId, string productId)
        {
            return await _context.FavoriteItems.IgnoreQueryFilters().AnyAsync(x => x.UserId == userId && x.ProductId == productId);
        }

        public async Task<FavoriteItem?> GetByUserAndProductAsync(Guid userId, string productId)
        {
            return await _context.FavoriteItems.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
        }

        public async Task<List<string>> GetUserFavoritesAsync(Guid userId)
        {
            return await _context.FavoriteItems
                .Where(x => x.UserId == userId)
                .Select(x => x.ProductId)
                .ToListAsync();
        }

        public async Task RemoveAsync(FavoriteItem favoriteItem)
        {
            _context.FavoriteItems.Remove(favoriteItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FavoriteItem favoriteItem)
        {
            _context.FavoriteItems.Update(favoriteItem);
            await _context.SaveChangesAsync();
        }
    }
}
