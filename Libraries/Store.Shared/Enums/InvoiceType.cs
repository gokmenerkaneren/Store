namespace Store.Shared.Enums
{
    /// <summary>
    /// Fatura kesim türünü belirleyen enum (Kurumsal / Bireysel).
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// Kurumsal Fatura (Şirket/Vergi No).
        /// </summary>
        Corporate = 1,

        /// <summary>
        /// Bireysel Fatura (TC Kimlik No).
        /// </summary>
        Individual = 2
    }
}
