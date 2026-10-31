using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Store.Favorite.Infrastructure.Context;
using Store.Favorite.Domain.Entities;

public class DbChecker {
    public static void Run() {
        var optionsBuilder = new DbContextOptionsBuilder<StoreFavoriteContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=StoreFavoriteDb;Username=postgres;Password=sa435558;");
        using var context = new StoreFavoriteContext(optionsBuilder.Options);
        var count = context.FavoriteItems.Count();
        Console.WriteLine("Count: " + count);
    }
}
