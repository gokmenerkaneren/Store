using FluentValidation;
using Store.Location.Application.Features.CQRS.Commands.WarehouseCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Linq;

namespace Store.Location.Application.Features.CQRS.Validators.WarehouseValidators
{
    public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
    {
        private readonly IRepository<Warehouse> _repository;

        public CreateWarehouseCommandValidator(IRepository<Warehouse> repository)
        {
            _repository = repository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Depo Adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Depo Adı 100 karakterden uzun olamaz.");

            // Business rule: Name and Code must be unique
            RuleFor(x => x)
                .MustAsync(async (command, cancellationToken) =>
                {
                    var name = (command.Name ?? "").Trim();
                    var code = (command.Code ?? "").Trim();
                    
                    var existing = await _repository.GetByFilterAsync(w => 
                        w.BranchId == command.BranchId && (
                            w.Name == name || w.Code == code
                        ));
                    
                    return existing == null;
                })
                .WithMessage("Aynı isme veya koda sahip depo bu şubede zaten mevcut.");
        }
    }
}
