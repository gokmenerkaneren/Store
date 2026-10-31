using System;

namespace Store.Location.Application.Features.CQRS.Commands.CityCommands
{
    public class RestoreCityCommand
    {
        public Guid Id { get; set; }
        public RestoreCityCommand(Guid id) => Id = id;
    }
}
