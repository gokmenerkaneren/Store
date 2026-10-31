using Store.Location.Application.Features.CQRS.Commands.BranchCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchHandlers
{
    public class CreateBranchCommandHandler
    {
        private readonly IRepository<Branch> _repository;

        public CreateBranchCommandHandler(IRepository<Branch> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateBranchCommand command)
        {
            var name = (command.Name ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Şube Adı boş bırakılamaz.");
            }

            var all = await _repository.GetAllAsync(includeDeleted: true);

            var existing = all.FirstOrDefault(b =>
                b.CompanyId == command.CompanyId && (
                    string.Equals((b.Name ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals((b.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase)
                )
            );

            if (existing != null)
            {
                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage("Şube Adı", existing.Name, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            var branch = new Branch
            {
                Name = name,
                BranchType = command.BranchType,
                CompanyId = command.CompanyId,
                ContactInfoId = command.ContactInfoId,
                GeoLocationCoordinates = command.GeoLocationCoordinates,
                Notes = command.Notes,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(branch);
        }
    }
}
