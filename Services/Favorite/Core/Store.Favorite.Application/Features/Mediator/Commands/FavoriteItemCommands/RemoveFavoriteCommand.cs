using MediatR;
using System;

namespace Store.Favorite.Application.Features.Mediator.Commands.FavoriteItemCommands
{
    public class RemoveFavoriteCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string ProductId { get; set; }
    }
}
