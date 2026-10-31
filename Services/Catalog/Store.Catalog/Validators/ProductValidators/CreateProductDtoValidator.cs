using FluentValidation;
using Store.Catalog.Dtos.ProductDtos;
using System;
using System.Linq;

namespace Store.Catalog.Validators.ProductValidators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı boş geçilemez.");

            RuleFor(x => x.ProductCode)
                .NotEmpty().WithMessage("Ürün kodu boş geçilemez.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Kategori mutlaka seçilmeli.")
                .NotEqual(Guid.Empty).WithMessage("Kategori mutlaka seçilmeli.");

            RuleFor(x => x.MeasurementId)
                .NotEmpty().WithMessage("Ölçü birimi mutlaka seçilmeli.")
                .NotEqual(Guid.Empty).WithMessage("Ölçü birimi mutlaka seçilmeli.");

            RuleFor(x => x.Barcode)
                .Must(b => string.IsNullOrEmpty(b) || (b.Length == 13 && b.All(char.IsDigit) && (b.StartsWith("868") || b.StartsWith("869"))))
                .WithMessage("Barkod boş bırakılabilir. Eğer girilecekse tam 13 rakamdan oluşmalı ve 868 veya 869 ile başlamalıdır.");

            RuleFor(x => x)
                .Must(x => x.Satis || x.Kiralama)
                .WithMessage("Ürün için Satış veya Kiralama seçeneklerinden en az biri seçilmelidir.");
        }
    }
}
