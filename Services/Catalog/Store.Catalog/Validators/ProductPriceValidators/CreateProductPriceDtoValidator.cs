using FluentValidation;
using Store.Catalog.Dtos.ProductPriceDtos;
using System;

namespace Store.Catalog.Validators.ProductPriceValidators
{
    public class CreateProductPriceDtoValidator : AbstractValidator<CreateProductPriceDto>
    {
        public CreateProductPriceDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Tutar 0'dan büyük olmalıdır.");

            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithMessage("Para birimi mutlaka seçilmeli.")
                .NotEqual(Guid.Empty).WithMessage("Para birimi mutlaka seçilmeli.");
        }
    }
}
