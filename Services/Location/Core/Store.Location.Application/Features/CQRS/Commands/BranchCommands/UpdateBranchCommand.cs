using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.BranchCommands
{
    public class UpdateBranchCommand
    {
        public Guid Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string BranchType { get; set; } = string.Empty;

        public Guid CompanyId { get; set; }
        public Guid? ContactInfoId { get; set; }

        [StringLength(100)]
        public string? GeoLocationCoordinates { get; set; }

        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
