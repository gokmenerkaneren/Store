using System;
using Store.Shared.Entities;

namespace Store.Order.Domain.Entities
{
    public class Address : BaseEntity
    { 
        public Guid UserId { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
