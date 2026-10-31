using MediatR;
using Store.Favorite.Application.Features.Mediator.Commands.FavoriteItemCommands;
using Store.Favorite.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Favorite.Application.Features.Mediator.Handlers.FavoriteItemHandlers
{
    public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand>
    {
        private readonly IFavoriteItemRepository _repository;

        public RemoveFavoriteCommandHandler(IFavoriteItemRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetByUserAndProductAsync(request.UserId, request.ProductId);
            if (item != null)
            {
                item.DeleteUserId = request.UserId;
                await _repository.RemoveAsync(item);
            }
        }
    }
}
