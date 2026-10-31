using Store.Location.Application.Features.CQRS.Queries.BultenAboneligiQueries;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace Store.Location.Application.Features.CQRS.Handlers.BultenAboneligiHandlers
{
    public class CheckBultenAboneligiQueryHandler
    {
        private readonly IRepository<BultenAboneligi> _repository;

        public CheckBultenAboneligiQueryHandler(IRepository<BultenAboneligi> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(CheckBultenAboneligiQuery query)
        {
            var value = await _repository.GetByFilterAsync(x => x.Email == query.Email && !x.IsDeleted);
            return value != null;
        }
    }
}
