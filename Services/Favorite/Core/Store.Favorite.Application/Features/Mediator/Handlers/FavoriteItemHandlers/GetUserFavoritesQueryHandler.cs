using MediatR;
using Store.Favorite.Application.Features.Mediator.Queries.FavoriteItemQueries;
using Store.Favorite.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Favorite.Application.Features.Mediator.Handlers.FavoriteItemHandlers
{
    public class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, List<string>>
    {
        private readonly IFavoriteItemRepository _repository;

        public GetUserFavoritesQueryHandler(IFavoriteItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<string>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetUserFavoritesAsync(request.UserId);
        }
    }
}
