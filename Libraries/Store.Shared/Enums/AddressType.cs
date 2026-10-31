namespace Store.Shared.Enums
{
    /// <summary>
    /// Bayi (Dealer) ve Şube (Branch) adreslerinde kullanılacak adres tipini belirleyen enum.
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Şirket veya bayinin yasal merkez adresi.
        /// </summary>
        Headquarters = 1,

        /// <summary>
        /// Fatura/muhasebe adresi.
        /// </summary>
        Billing = 2,

        /// <summary>
        /// Sevkiyat ve teslimat adresi.
        /// </summary>
        Shipping = 3
    }
}
