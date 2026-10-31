using Store.Catalog.Entities;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Dtos.CurrencyDtos
{
    public class GetByIdCurrencyDto : BaseEntity
    {
        public string Code { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty; 
        public string Symbol { get; set; } = string.Empty;
    }
}

