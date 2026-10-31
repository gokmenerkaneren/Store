namespace Store.Shared.Enums
{
    /// <summary>
    /// Ürün fiyatlandırmasının hangi satış veya kiralama stratejisine ait olduğunu belirten enum.
    /// </summary>
    public enum PriceType
    {
        /// <summary>
        /// Ürünün firmaya geliş/üretim maliyeti.
        /// </summary>
        Cost = 1,

        /// <summary>
        /// Son kullanıcıya (B2C) sunulan perakende satış fiyatı.
        /// </summary>
        Retail = 2,

        /// <summary>
        /// Ürünün son kullanıcılar (B2C) için belirlenen günlük/dönemlik kiralama fiyatı.
        /// </summary>
        Rental = 3,

        /// <summary>
        /// Bayilere veya toptancılara (B2B) sunulan özel satış fiyatı.
        /// </summary>
        Wholesale = 4,

        /// <summary>
        /// Bayilere veya toptancılara (B2B) sunulan özel kiralama fiyatı.
        /// </summary>
        Wholerental = 5
    }
}
