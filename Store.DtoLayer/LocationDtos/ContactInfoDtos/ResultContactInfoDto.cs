using System;

namespace Store.DtoLayer.LocationDtos.ContactInfoDtos
{
    public class ResultContactInfoDto
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
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }
}
