using Store.Location.Application.Features.CQRS.Queries.BultenAboneligiQueries;
using Store.Location.Application.Features.CQRS.Results.BultenAboneligiResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.BultenAboneligiHandlers
{
    public class GetBultenAboneligiByIdQueryHandler
    {
        private readonly IRepository<BultenAboneligi> _repository;

        public GetBultenAboneligiByIdQueryHandler(IRepository<BultenAboneligi> repository)
        {
            _repository = repository;
        }

        public async Task<GetBultenAboneligiByIdQueryResult> Handle(GetBultenAboneligiByIdQuery query)
        {
            var values = await _repository.GetByIdAsync(query.Id);
            return new GetBultenAboneligiByIdQueryResult
            {
                Id = values.Id,
                Email = values.Email
            };
        }
    }
}
