using System;

namespace Store.DtoLayer.LocationDtos.ContactInfoDtos
{
    public class UpdateContactInfoDto
    {
        public Guid Id { get; set; }
        public string CompanyEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string AuthorizedPersonName { get; set; } = string.Empty;
        public string AuthorizedPersonTitle { get; set; } = string.Empty;
        public string AuthorizedPersonPhone { get; set; } = string.Empty;
        public string AuthorizedPersonEmail { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
