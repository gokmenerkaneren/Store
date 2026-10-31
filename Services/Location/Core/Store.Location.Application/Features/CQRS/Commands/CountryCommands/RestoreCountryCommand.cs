using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Commands.CountryCommands
{
    public class RestoreCountryCommand
    {
        public Guid Id { get; set; }
        public RestoreCountryCommand(Guid id) => Id = id;
    }
}
