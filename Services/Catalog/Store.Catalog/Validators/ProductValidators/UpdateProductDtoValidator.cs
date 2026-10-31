using FluentValidation;
using Store.Catalog.Dtos.ProductDtos;
using System;

namespace Store.Catalog.Validators.ProductValidators
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
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

            RuleFor(x => x.BrandId)
                .NotEmpty().WithMessage("Marka mutlaka seçilmeli.")
                .NotEqual(Guid.Empty).WithMessage("Marka mutlaka seçilmeli.");

            RuleFor(x => x.Description)
                .MinimumLength(5).WithMessage("Genişletilmiş açıklama en az 5 karakter olmalıdır.");

            RuleFor(x => x.Barcode)
                .Must(b => string.IsNullOrEmpty(b) || (b.Length == 13 && b.All(char.IsDigit) && (b.StartsWith("868") || b.StartsWith("869"))))
                .WithMessage("Barkod boş bırakılabilir. Eğer girilecekse tam 13 rakamdan oluşmalı ve 868 veya 869 ile başlamalıdır.");

            RuleFor(x => x.Tags)
                .Must(t => t != null && t.Count > 0)
                .WithMessage("Etikete en az bir etiket verisi girilmeli.");

            RuleFor(x => x.Notes)
                .NotEmpty().WithMessage("Öne çıkan özellikler boş geçilemez.");

            RuleFor(x => x)
                .Must(x => x.Satis || x.Kiralama)
                .WithMessage("Ürün için Satış veya Kiralama seçeneklerinden en az biri seçilmelidir.");
        }
    }
}
