using MediatR;
using System;
using System.Collections.Generic;

namespace Store.Favorite.Application.Features.Mediator.Queries.FavoriteItemQueries
{
    public class GetUserFavoritesQuery : IRequest<List<string>>
    {
        public Guid UserId { get; set; }
        public GetUserFavoritesQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
