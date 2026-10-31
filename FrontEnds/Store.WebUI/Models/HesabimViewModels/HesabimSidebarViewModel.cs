using System;

namespace Store.WebUI.Models.HesabimViewModels
{
    public class HesabimSidebarViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ActiveTab { get; set; } = "KullaniciBilgileri"; // KullaniciBilgileri, AdresBilgileri, KayitliKartlarim, SifreDegistirme, Siparislerim, Favorilerim, Mesajlarim, IndirimKuponlarim
        public int FavoritesCount { get; set; }
        public int OrdersCount { get; set; }
    }
}

