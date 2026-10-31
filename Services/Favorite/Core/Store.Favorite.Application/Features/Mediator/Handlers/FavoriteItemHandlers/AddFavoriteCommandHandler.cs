using MediatR;
using Store.Favorite.Application.Features.Mediator.Commands.FavoriteItemCommands;
using Store.Favorite.Application.Interfaces;
using Store.Favorite.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Favorite.Application.Features.Mediator.Handlers.FavoriteItemHandlers
{
    public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand>
    {
        private readonly IFavoriteItemRepository _repository;

        public AddFavoriteCommandHandler(IFavoriteItemRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
        {
            var existingItem = await _repository.GetByUserAndProductAsync(request.UserId, request.ProductId);
            
            if (existingItem != null)
            {
                if (existingItem.IsDeleted)
                {
                    existingItem.IsDeleted = false;
                    existingItem.IsActive = true;
                    existingItem.UpdateUserId = request.UserId;
                    await _repository.UpdateAsync(existingItem);
                }
            }
            else
            {
                var favorite = new FavoriteItem
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    InsertUserId = request.UserId
                };
                await _repository.AddAsync(favorite);
            }
        }
    }
}
