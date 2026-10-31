using MediatR;
using Store.Order.Application.Features.Mediator.Queries.OrderingQueries;
using Store.Order.Application.Features.Mediator.Results.OrderingResults;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.Mediator.Handlers.OrderingHandlers
{
    public class GetOrderingQueryHandler : IRequestHandler<GetOrderingQuery, List<GetOrderingQueryResult>>
    {
        private readonly IRepository<Ordering> repository;

        public GetOrderingQueryHandler(IRepository<Ordering> repository)
        {
            this.repository = repository;
        }

        public async Task<List<GetOrderingQueryResult>> Handle(GetOrderingQuery request, CancellationToken cancellationToken)
        {
            var values = await repository.GetAllAsync();
            return values.Select(x => new GetOrderingQueryResult
            {
                Id = x.Id,
                OrderDate = x.OrderDate,
                TotalPrice = x.TotalPrice,
                UserId = x.UserId,
                Status = x.Status
            }).ToList();
        }
    }
}
