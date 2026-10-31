using FluentValidation;
using Store.Favorite.Application.Features.Mediator.Commands.FavoriteItemCommands;

namespace Store.Favorite.Application.Features.Mediator.Validators
{
    public class RemoveFavoriteCommandValidator : AbstractValidator<RemoveFavoriteCommand>
    {
        public RemoveFavoriteCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Kullanıcı kimliği boş olamaz.");
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Ürün kimliği boş olamaz.");
        }
    }
}
