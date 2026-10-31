using System;

namespace Store.Location.Application.Features.CQRS.Queries.BultenAboneligiQueries
{
    public class GetBultenAboneligiByIdQuery
    {
        public Guid Id { get; set; }
        public GetBultenAboneligiByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
