using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands
{
    public class UpdateNeighborhoodCommand
    {
        public Guid Id { get; set; }

        [Required, StringLength(150)]
        public string NeighborhoodName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PostalCode { get; set; }

        public Guid DistrictId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
