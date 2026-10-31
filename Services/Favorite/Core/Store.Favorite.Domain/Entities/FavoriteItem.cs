using Store.Shared.Entities;
using System;

namespace Store.Favorite.Domain.Entities
{
    public class FavoriteItem : BaseEntity
    {
        public Guid UserId { get; set; }
        public string ProductId { get; set; }
    }
}

