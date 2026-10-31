using System;

namespace Store.Location.Application.Features.CQRS.Commands.CompanyCommands
{
    public class RestoreCompanyCommand
    {
        public Guid Id { get; set; }
        public RestoreCompanyCommand(Guid id) => Id = id;
    }
}
