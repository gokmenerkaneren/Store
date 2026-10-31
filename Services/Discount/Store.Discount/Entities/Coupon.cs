using Store.Shared.Entities;
using System;

namespace Store.Discount.Entities
{
    public class Coupon : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public int Rate { get; set; }
        public DateTime ValidDate { get; set; }
    }
}
