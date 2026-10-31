using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// İlçelere bağlı mahalleleri ve posta kodlarını tutar. İnce nakliye (Desi/Mesafe) hesabı için kullanılır.
    public class Neighborhood : BaseEntity
    {
        /// Mahallenin resmi adı (Örn: Bostanlı Mahallesi).
        [Required, StringLength(150)]
        public string NeighborhoodName { get; set; } = string.Empty;

        /// Mahalleye ait 5 haneli resmi posta kodu (Örn: 35590).
        [StringLength(20)]
        public string? PostalCode { get; set; }

        /// Mahallenin bağlı olduğu ilçenin benzersiz kimliği (Foreign Key).
        public Guid DistrictId { get; set; }

        /// Mahallenin bağlı olduğu İlçe nesnesine doğrudan erişim (Navigation Property).
        public District? District { get; set; }
    }

}
