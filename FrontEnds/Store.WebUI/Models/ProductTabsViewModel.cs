using System.Collections.Generic;

namespace Store.WebUI.Models
{
    // "En Cok Satanlar" ve "Yeni Urunler" sekmelerini
    // ayni View'da birlikte tasimak icin kullanilan ViewModel.
    public class ProductTabsViewModel
    {
        // En Cok Satanlar sekmesindeki urunler
        public List<ProductListViewModel> BestSellers { get; set; } = new();

        // Yeni Urunler sekmesindeki urunler
        public List<ProductListViewModel> NewProducts { get; set; } = new();
    }
}
