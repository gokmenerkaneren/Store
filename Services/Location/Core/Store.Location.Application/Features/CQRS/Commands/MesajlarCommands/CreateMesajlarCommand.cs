using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.MesajlarCommands
{
    public class CreateMesajlarCommand
    {
        public string NameSurname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
