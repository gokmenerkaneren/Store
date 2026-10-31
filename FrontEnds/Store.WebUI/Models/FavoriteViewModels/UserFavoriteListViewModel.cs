using System;
using System.Collections.Generic;

namespace Store.WebUI.Models.FavoriteViewModels
{
    public class UserFavoriteListViewModel
    {
        public List<FavoriteItemViewModel> Items { get; set; } = new List<FavoriteItemViewModel>();
        
        // Sayfalama için
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }

    public class FavoriteItemViewModel
    {
        // Temel Bilgiler
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        
        // B2B & B2C Fiyatlandırma
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string CurrencySymbol { get; set; }
        
        // Stok ve Durum
        public bool IsActive { get; set; } // Ürün satışta mı? (Pasif/Silinmiş)
        public bool IsInStock { get; set; } // Stok var mı?
        public bool HasVariants { get; set; } // Varyantlı ürün mü?
        
        // Özel Etiketler
        public bool IsPriceDropped { get; set; } // Fiyatı Düştü rozeti
        
        // Sepet Miktarı
        public int DefaultQuantity { get; set; } = 1;
        
        // Favoriye Eklendiği Tarih (Sıralama veya rozet hesaplaması için)
        public DateTime AddedAt { get; set; }
    }
}
