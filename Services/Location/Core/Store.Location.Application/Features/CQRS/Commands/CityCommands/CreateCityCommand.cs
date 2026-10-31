using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.CityCommands
{
    public class CreateCityCommand
    {
        [Required, StringLength(100)]
        public string CityName { get; set; } = string.Empty;

        [StringLength(10)]
        public string? PlateCode { get; set; }

        public Guid CountryId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
