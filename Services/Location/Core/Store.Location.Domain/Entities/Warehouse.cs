using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// <summary>
    /// Ürünlerin fiziksel olarak stoklandığı lojistik depolarını tutar.
    /// Adres ve iletişim bilgileri ContactInfo FK üzerinden normalize edilmiştir.
    /// </summary>
    public class Warehouse : BaseEntity
    {
        /// Deponun açık adı (Örn: İzmir Bornova Lojistik Merkezi).
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        /// Depoya atanan kısa işletme kodu (Örn: WH-01).
        [StringLength(50)]
        public string? Code { get; set; }

        /// Bu deponun sistemin varsayılan (ana) deposu olup olmadığını belirtir.
        public bool IsDefault { get; set; }

        // ==========================================
        // RELATIONS (İLİŞKİLER)
        // ==========================================

        /// Bu deponun eğer bir şubeye bağlıysa ilgili şubenin ID'si (Foreign Key).
        public Guid? BranchId { get; set; }

        /// Deponun bağlı olduğu Şube nesnesine erişim (Navigation Property).
        public Branch? Branch { get; set; }

        /// Deponun iletişim ve sorumlu kişi bilgilerini tutan kaydın ID'si (Foreign Key).
        /// Eski düz metin Address ve City alanlarının yerini almıştır — normalize edildi.
        public Guid? ContactInfoId { get; set; }

        /// Deponun iletişim nesnesine erişim (Navigation Property).
        public ContactInfo? ContactInfo { get; set; }
    }
}

