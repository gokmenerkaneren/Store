using System;

namespace Store.DtoLayer.OrderDtos
{
    public class CreateOrderAddressDto
    {
        public Guid UserId { get; set; }
        public string Country { get; set; } = "Türkiye";
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}

