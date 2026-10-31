using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.CountryCommands
{
    public class CreateCountryCommand
    {
        [Required, StringLength(100)]
        public string CountryName { get; set; } = string.Empty;

        [StringLength(5)]
        public string? CountryCode { get; set; }

        [StringLength(10)]
        public string? UPhoneCode { get; set; }

        public string? Notes { get; set; }
    }
}
