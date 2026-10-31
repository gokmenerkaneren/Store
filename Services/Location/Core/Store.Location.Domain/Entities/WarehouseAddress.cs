using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// <summary>
    /// Şirketin depolarının (Warehouse) fiziksel adres kayıtlarını tutar.
    /// AddressBase'den türeyerek Country/City/District/Neighborhood lokasyon hiyerarşisini miras alır.
    /// </summary>
    public class WarehouseAddress : AddressBase
    {
        /// <summary>
        /// Bu adresin hangi Depoya ait olduğunu belirten kimlik (Foreign Key).
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// Adresin bağlı olduğu Depo nesnesine erişim (Navigation Property).
        /// </summary>
        public Warehouse? Warehouse { get; set; }
        
        /// <summary>
        /// Depo teslimat ve lojistik tır/kamyon yanaşma rampa talimatı.
        /// </summary>
        [StringLength(500)]
        public string? LoadingDockInstructions { get; set; }
    }
}
