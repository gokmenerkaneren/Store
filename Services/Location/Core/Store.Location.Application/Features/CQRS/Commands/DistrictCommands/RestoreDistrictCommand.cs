using System;

namespace Store.Location.Application.Features.CQRS.Commands.DistrictCommands
{
    public class RestoreDistrictCommand
    {
        public Guid Id { get; set; }
        public RestoreDistrictCommand(Guid id) => Id = id;
    }
}
