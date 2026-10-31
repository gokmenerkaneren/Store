using Store.Location.Application.Features.CQRS.Commands.CompanyCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CompanyHandlers
{
    public class SoftDeleteCompanyCommandHandler
    {
        private readonly IRepository<Company> _repository;

        public SoftDeleteCompanyCommandHandler(IRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteCompanyCommand command)
        {
            var company = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (company == null) return;

            await _repository.SoftDeleteAsync(company);
        }
    }
}
