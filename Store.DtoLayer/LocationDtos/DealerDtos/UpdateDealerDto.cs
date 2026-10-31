using System;
using System.ComponentModel.DataAnnotations;

namespace Store.DtoLayer.LocationDtos.DealerDtos
{
    public class UpdateDealerDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Lütfen bu alanı doldurun.")]
        [MinLength(5, ErrorMessage = "Ticari Unvan en az 5 karakter olmalıdır.")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen bu alanı doldurun.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Vergi Numarası tam olarak 11 rakamdan oluşmalıdır.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Vergi Numarası sadece sayılardan oluşmalıdır.")]
        public string TaxNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen bu alanı doldurun.")]
        [MinLength(5, ErrorMessage = "Vergi Dairesi en az 5 karakter olmalıdır.")]
        public string TaxOffice { get; set; } = string.Empty;

        public decimal DiscountRate { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public Guid? ContactInfoId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
