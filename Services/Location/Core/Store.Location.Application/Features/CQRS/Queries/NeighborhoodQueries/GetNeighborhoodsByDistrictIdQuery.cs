using System;

namespace Store.Location.Application.Features.CQRS.Queries.NeighborhoodQueries
{
    public class GetNeighborhoodsByDistrictIdQuery
    {
        public Guid DistrictId { get; set; }
        public bool ShowDeleted { get; set; }

        public GetNeighborhoodsByDistrictIdQuery(Guid districtId, bool showDeleted = false)
        {
            DistrictId = districtId;
            ShowDeleted = showDeleted;
        }
    }
}
