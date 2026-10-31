using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.BultenAboneligiCommands
{
    public class CreateBultenAboneligiCommand
    {
        public string Email { get; set; }
    }
}
