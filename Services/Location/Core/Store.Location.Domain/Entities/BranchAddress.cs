using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// Şirketin kendisine ait şube veya lojistik depolarının adres detaylarını tutar.
    public class BranchAddress : AddressBase
    {
        /// Bu adresin hangi Şubeye/Depoya ait olduğunu belirten kimlik (Foreign Key).
        public Guid BranchId { get; set; }

        /// Büyük kargo/lojistik firmaları için Tır/Rampa yanaşma ve boşaltma talimatı.
        [StringLength(500)]
        public string? LoadingDockInstructions { get; set; }
    }

}
