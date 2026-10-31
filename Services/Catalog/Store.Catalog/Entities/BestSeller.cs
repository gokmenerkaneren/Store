using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;

namespace Store.Catalog.Entities
{
    public class BestSeller : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }

        public int OrderIndex { get; set; }
    }
}
