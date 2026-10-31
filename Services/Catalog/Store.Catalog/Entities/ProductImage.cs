using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Entities
{
    /// Ürüne ait resimleri ve bu resimlerin vitrindeki sırasını tutar.
    public class ProductImage : BaseEntity
    {
        /// Bu resmin bağlı olduğu ana Ürünün ID'si.
        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }

        /// Resmin bulut sunucusu (AWS S3 vb.) veya CDN üzerindeki erişim URL yolu.
        public string ImageUrl { get; set; } = string.Empty;

        /// Eğer true ise, bu resim ürünün site vitrinindeki kapak (ana) fotoğrafıdır.
        public bool IsMain { get; set; }

        /// Birden fazla resim olduğunda galeri içindeki gösterim sırasını belirtir.
        public int DisplayOrder { get; set; }

        /// Resim yüklenemediğinde veya görme engelli kullanıcılar için resmi anlatan SEO destekli metin.
        public string AltText { get; set; } = string.Empty;
    }
}

