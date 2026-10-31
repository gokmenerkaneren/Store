using System;

namespace Store.Location.Application.Features.CQRS.Queries.MesajlarQueries
{
    public class GetMesajlarByIdQuery
    {
        public Guid Id { get; set; }
        public GetMesajlarByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
