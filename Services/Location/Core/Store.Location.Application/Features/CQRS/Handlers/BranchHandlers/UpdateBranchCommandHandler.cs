using Store.Location.Application.Features.CQRS.Commands.BranchCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchHandlers
{
    public class UpdateBranchCommandHandler
    {
        private readonly IRepository<Branch> _repository;

        public UpdateBranchCommandHandler(IRepository<Branch> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateBranchCommand command)
        {
            var name = (command.Name ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Şube Adı boş bırakılamaz.");
            }

            var all = await _repository.GetAllAsync(includeDeleted: true);

            var existing = all.FirstOrDefault(b =>
                b.Id != command.Id && b.CompanyId == command.CompanyId && (
                    string.Equals((b.Name ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals((b.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase)
                )
            );

            if (existing != null)
            {
                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage("Şube Adı", existing.Name, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            var branch = await _repository.GetByIdAsync(command.Id);
            branch.Name = name;
            branch.BranchType = command.BranchType;
            branch.CompanyId = command.CompanyId;
            branch.ContactInfoId = command.ContactInfoId;
            branch.GeoLocationCoordinates = command.GeoLocationCoordinates;
            branch.Notes = command.Notes;
            branch.IsActive = command.IsActive;

            await _repository.UpdateAsync(branch);
        }
    }
}
