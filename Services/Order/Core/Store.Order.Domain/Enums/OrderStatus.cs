namespace Store.Order.Domain.Enums
{
    /// <summary>
    /// Sipariş Yaşam Döngüsü Durumları (İ3 Durum Makinesi)
    /// </summary>
    public enum OrderStatus
    {
        Pending = 0,       // Sipariş alındı / Beklemede
        Processing = 1,    // Sipariş hazırlanıyor / İşleniyor
        Shipped = 2,       // Kargoya verildi
        Delivered = 3,     // Teslim edildi
        Cancelled = 4,     // İptal edildi
        Completed = 5,     // Tamamlandı
        Returned = 6       // İade edildi
    }
}
