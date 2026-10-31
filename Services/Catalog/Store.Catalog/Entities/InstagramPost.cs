using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;

namespace Store.Catalog.Entities
{
    public class InstagramPost : BaseEntity
    {
        public string ImageUrl { get; set; }
        public string PostUrl { get; set; }
        public int OrderIndex { get; set; }
    }
}
