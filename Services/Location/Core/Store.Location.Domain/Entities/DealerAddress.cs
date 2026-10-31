using System;
using Store.Shared.Enums;
using System.Collections.Generic;
using Store.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using Store.Shared.Enums;
using System.Linq;
using Store.Shared.Enums;
using System.Text;
using Store.Shared.Enums;
using System.Threading.Tasks;
using Store.Shared.Enums;

namespace Store.Location.Domain.Entities
{
    /// <summary>
    /// Bayi (Dealer) firmalarına ait fiziksel adres kayıtlarını tutar.
    /// AddressBase'den türeyerek Country/City/District/Neighborhood lokasyon hiyerarşisini miras alır.
    /// Bir bayinin farklı tipte (Merkez, Fatura, Sevkiyat) birden fazla adresi olabilir.
    /// </summary>
    public class DealerAddress : AddressBase
    {
        /// Bu adresin hangi Bayiye ait olduğunu belirten kimlik (Foreign Key).
        public Guid DealerId { get; set; }

        /// Adresin bağlı olduğu Bayi nesnesine erişim (Navigation Property).
        public Dealer? Dealer { get; set; }

        /// Adresin türünü belirtir: Merkez (1), Fatura (2), Sevkiyat (3).
        /// EF Core Fluent API'de HasConversion<int>() ile integer olarak saklanır.
        public AddressType AddressType { get; set; } = AddressType.Headquarters;
    }
}
