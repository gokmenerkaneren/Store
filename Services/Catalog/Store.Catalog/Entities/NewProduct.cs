using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Store.Shared.Entities;
using System;

namespace Store.Catalog.Entities
{
    public class NewProduct : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }
        
        public int OrderIndex { get; set; }
    }
}
