using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.WarehouseCommands
{
    public class UpdateWarehouseCommand
    {
        public Guid Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Code { get; set; }

        public bool IsDefault { get; set; }
        public Guid? BranchId { get; set; }

        /// Deponun iletişim ve sorumlu kişi bilgisi (yeni FK — eski string Address/City/Phone alanlarının yerine).
        public Guid? ContactInfoId { get; set; }

        public bool IsActive { get; set; }
        public string? Notes { get; set; }

        public Store.DtoLayer.LocationDtos.WarehouseDtos.WarehouseAddressDto? Address { get; set; }
    }
}
