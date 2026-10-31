using MediatR;
using Store.Order.Application.Features.Mediator.Results.OrderingResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.Mediator.Queries.OrderingQueries
{
    public class GetOrderingByIdQuery : IRequest<GetOrderingByIdQueryResult>
    {
        public Guid Id { get; set; }
        public GetOrderingByIdQuery(Guid id)
        {
            Id = id;
        }

        
    }
}


