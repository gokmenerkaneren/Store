namespace Store.Shared.Enums
{
    /// <summary>
    /// Ürün bedeninin hangi ölçüm sistemine ait olduğunu belirten enum.
    /// Aynı Size koleksiyonunda farklı tipte bedenlerin (S/M/L, 36/38, 34/31)
    /// birlikte tutulabilmesini ve doğru gruplandırılmasını sağlar.
    /// </summary>
    public enum SizeType
    {
        /// <summary>
        /// Harf tabanlı giysi bedeni. Örn: XS, S, M, L, XL, XXL
        /// </summary>
        Clothing = 1,

        /// <summary>
        /// Sayısal giysi bedeni. Örn: 36, 38, 40, 42, 44
        /// </summary>
        Number = 2,

        /// <summary>
        /// İki boyutlu kot/pantolon bedeni (Bel x Boy). 
        /// PrimaryValue = Bel (ör: 34), SecondaryValue = Bacak Boyu (ör: 31)
        /// </summary>
        Denim = 3,

        /// <summary>
        /// Ayakkabı numarası. Örn: 38, 39, 40, 41, 42
        /// </summary>
        Shoe = 4
    }
}
