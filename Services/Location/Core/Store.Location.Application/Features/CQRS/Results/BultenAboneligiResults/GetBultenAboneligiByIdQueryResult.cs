using System;

namespace Store.Location.Application.Features.CQRS.Results.BultenAboneligiResults
{
    public class GetBultenAboneligiByIdQueryResult
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}
