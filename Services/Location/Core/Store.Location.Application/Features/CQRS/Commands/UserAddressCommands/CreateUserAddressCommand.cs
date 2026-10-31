using System;
using System.ComponentModel.DataAnnotations;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Commands.UserAddressCommands
{
    public class CreateUserAddressCommand
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

        public Guid CustomerId { get; set; }
        public InvoiceType InvoiceType { get; set; }

        [StringLength(11)]
        public string? IdentityNumber { get; set; }

        [StringLength(100)]
        public string? TaxOffice { get; set; }

        [StringLength(50)]
        public string? TaxNumber { get; set; }

        public Guid? CompanyId { get; set; }
        public bool IsDefault { get; set; } = false;
        public string? Notes { get; set; }
    }
}
