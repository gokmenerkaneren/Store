using MediatR;
using Store.Order.Application.Features.Mediator.Queries.OrderingQueries;
using Store.Order.Application.Features.Mediator.Results.OrderingResults;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.Mediator.Handlers.OrderingHandlers
{
    public class GetOrderingByIdQueryHandler : IRequestHandler<GetOrderingByIdQuery, GetOrderingByIdQueryResult>
    {
        private readonly IRepository<Ordering> _repository;

        public GetOrderingByIdQueryHandler(IRepository<Ordering> repository)
        {
            _repository = repository;
        }

        public async Task<GetOrderingByIdQueryResult> Handle(GetOrderingByIdQuery request, CancellationToken cancellationToken)
        {
            var values = await _repository.GetByIdAsync(request.Id);
            if (values == null)
            {
                return null;
            }

            return new GetOrderingByIdQueryResult
            {
                Id = values.Id,
                UserId = values.UserId,
                TotalPrice = values.TotalPrice,
                OrderDate = values.OrderDate,
                Status = values.Status
            };
        }
    }
}
