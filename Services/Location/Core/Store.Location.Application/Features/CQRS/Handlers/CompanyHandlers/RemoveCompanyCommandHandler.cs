using Store.Location.Application.Features.CQRS.Commands.CompanyCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CompanyHandlers
{
    public class RemoveCompanyCommandHandler
    {
        private readonly IRepository<Company> _repository;

        public RemoveCompanyCommandHandler(IRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveCompanyCommand command)
        {
            var company = await _repository.GetByIdAsync(command.Id);
            if (company != null)
            {
                await _repository.SoftDeleteAsync(company);
            }
        }
    }
}
