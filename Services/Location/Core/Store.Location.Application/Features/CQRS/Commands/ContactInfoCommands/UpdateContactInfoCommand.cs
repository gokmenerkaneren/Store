using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands
{
    public class UpdateContactInfoCommand
    {
        public Guid Id { get; set; }

        // Kurumsal İletişim
        [StringLength(150)]
        public string CompanyEmail { get; set; } = string.Empty;

        [StringLength(30)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(30)]
        public string Fax { get; set; } = string.Empty;

        [StringLength(100)]
        public string Website { get; set; } = string.Empty;

        // Yetkili Kişi Bilgileri
        [StringLength(100)]
        public string AuthorizedPersonName { get; set; } = string.Empty;

        [StringLength(100)]
        public string AuthorizedPersonTitle { get; set; } = string.Empty;

        [StringLength(30)]
        public string AuthorizedPersonPhone { get; set; } = string.Empty;

        [StringLength(150)]
        public string AuthorizedPersonEmail { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
