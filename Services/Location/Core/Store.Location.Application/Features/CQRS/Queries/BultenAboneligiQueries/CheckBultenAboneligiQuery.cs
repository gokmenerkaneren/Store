using System;

namespace Store.Location.Application.Features.CQRS.Queries.BultenAboneligiQueries
{
    public class CheckBultenAboneligiQuery
    {
        public string Email { get; set; }

        public CheckBultenAboneligiQuery(string email)
        {
            Email = email;
        }
    }
}
