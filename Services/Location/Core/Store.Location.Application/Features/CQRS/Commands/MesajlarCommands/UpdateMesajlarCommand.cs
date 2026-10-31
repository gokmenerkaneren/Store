using System;

namespace Store.Location.Application.Features.CQRS.Commands.MesajlarCommands
{
    public class UpdateMesajlarCommand
    {
        public Guid Id { get; set; }
        public bool IsRead { get; set; }
    }
}
