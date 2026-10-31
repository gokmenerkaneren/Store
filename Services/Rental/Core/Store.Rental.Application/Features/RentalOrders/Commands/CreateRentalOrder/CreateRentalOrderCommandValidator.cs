using FluentValidation;

namespace Store.Rental.Application.Features.RentalOrders.Commands.CreateRentalOrder
{
    public class CreateRentalOrderCommandValidator : AbstractValidator<CreateRentalOrderCommand>
    {
        public CreateRentalOrderCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Kullanıcı ID boş olamaz.");
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Başlangıç tarihi seçilmelidir.");
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("Bitiş tarihi, başlangıç tarihinden sonra olmalıdır.");
            RuleFor(x => x.Items).NotEmpty().WithMessage("Kiralama listesi boş olamaz.");
        }
    }
}
