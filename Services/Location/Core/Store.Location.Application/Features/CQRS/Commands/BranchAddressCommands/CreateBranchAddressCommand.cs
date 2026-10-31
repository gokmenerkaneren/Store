using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands
{
    public class CreateBranchAddressCommand
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string ContactFullName { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string AddressDetail { get; set; } = string.Empty;

        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }
        public Guid BranchId { get; set; }

        [StringLength(500)]
        public string? LoadingDockInstructions { get; set; }

        public string? Notes { get; set; }
    }
}
