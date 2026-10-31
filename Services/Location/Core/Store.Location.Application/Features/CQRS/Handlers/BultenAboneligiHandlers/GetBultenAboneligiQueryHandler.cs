using Store.Location.Application.Features.CQRS.Queries.BultenAboneligiQueries;
using Store.Location.Application.Features.CQRS.Results.BultenAboneligiResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.BultenAboneligiHandlers
{
    public class GetBultenAboneligiQueryHandler
    {
        private readonly IRepository<BultenAboneligi> _repository;

        public GetBultenAboneligiQueryHandler(IRepository<BultenAboneligi> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetBultenAboneligiQueryResult>> Handle(bool showDeleted = false)
        {
            var values = await _repository.GetAllAsync(showDeleted);
            return values.Select(x => new GetBultenAboneligiQueryResult
            {
                Id = x.Id,
                Email = x.Email
            }).ToList();
        }
    }
}
