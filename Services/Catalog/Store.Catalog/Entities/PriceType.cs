using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;

namespace Store.Catalog.Entities
{
    public class PriceType : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
