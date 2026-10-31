using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Commands.CountryCommands
{
    public class SoftDeleteCountryCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteCountryCommand(Guid id) => Id = id;

    }
}
