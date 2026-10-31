using System;

namespace Store.Location.Application.Features.CQRS.Commands.BultenAboneligiCommands
{
    public class UpdateBultenAboneligiCommand
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}
