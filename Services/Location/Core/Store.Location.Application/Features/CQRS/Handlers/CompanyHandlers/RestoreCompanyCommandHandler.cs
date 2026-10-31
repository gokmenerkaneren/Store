using Store.Location.Application.Features.CQRS.Commands.CompanyCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.CompanyHandlers
{
    public class RestoreCompanyCommandHandler
    {
        private readonly IRepository<Company> _repository;

        public RestoreCompanyCommandHandler(IRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreCompanyCommand command)
        {
            var company = await _repository.GetByIdAsync(command.Id);
            if (company == null) return;

            await _repository.RestoreAsync(company);
        }
    }
}
