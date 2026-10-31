using System;

namespace Store.DtoLayer.DiscountDtos
{
    public class ResultCouponDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int Rate { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidDate { get; set; }
    }
}
