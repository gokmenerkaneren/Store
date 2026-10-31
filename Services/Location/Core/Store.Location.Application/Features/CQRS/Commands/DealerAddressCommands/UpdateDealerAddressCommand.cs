using System.ComponentModel.DataAnnotations;
using Store.Shared.Enums;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Commands.DealerAddressCommands
{
    public class UpdateDealerAddressCommand
    {
        public Guid Id { get; set; }

        public Guid DealerId { get; set; }

        /// Adres tipi: Headquarters=1, Billing=2, Shipping=3
        public AddressType AddressType { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string ContactFullName { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string AddressDetail { get; set; } = string.Empty;

        // Lokasyon (Dropdown seçimli)
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }

        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
