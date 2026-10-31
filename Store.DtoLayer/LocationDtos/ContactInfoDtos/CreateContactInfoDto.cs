using System;

namespace Store.DtoLayer.LocationDtos.ContactInfoDtos
{
    public class CreateContactInfoDto
    {
        public string CompanyEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string AuthorizedPersonName { get; set; } = string.Empty;
        public string AuthorizedPersonTitle { get; set; } = string.Empty;
        public string AuthorizedPersonPhone { get; set; } = string.Empty;
        public string AuthorizedPersonEmail { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
