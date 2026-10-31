using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// Şehirlere bağlı ilçeleri tutar.		
    public class District : BaseEntity
    {
        /// İlçenin adı (Örn: Karşıyaka, Kadıköy).		
        [Required, StringLength(100)]
        public string DistrictName { get; set; } = string.Empty;

        /// İlçenin bağlı olduğu şehrin benzersiz kimliği (Foreign Key).		
        public Guid CityId { get; set; }

        /// İlçenin bağlı olduğu Şehir nesnesine doğrudan erişim (Navigation Property).		
        public City? City { get; set; }

        /// Bu ilçeye bağlı olan mahallelerin listesi (Bire-Çok İlişki Navigation Property).		
        public ICollection<Neighborhood> Neighborhoods { get; set; } = new List<Neighborhood>();
    }

}
