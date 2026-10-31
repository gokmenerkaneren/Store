using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.DistrictCommands
{
    public class UpdateDistrictCommand
    {
        public Guid Id { get; set; }

        [Required, StringLength(100)]
        public string DistrictName { get; set; } = string.Empty;

        public Guid CityId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
