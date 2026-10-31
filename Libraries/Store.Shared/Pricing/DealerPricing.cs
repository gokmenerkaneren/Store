using System;

namespace Store.Shared.Pricing
{
    public enum DealerCreditValidationResult
    {
        SufficientCredit,
        InsufficientCredit
    }

    /// <summary>
    /// Bayi iskontosu ve kredi limiti kuralları (saf fonksiyonlar).
    /// </summary>
    public static class DealerPricing
    {
        /// <summary>
        /// İskonto tutarını hesaplar. 1 veya daha büyük oranlar yüzde (15 = %15, 1 = %1), 1'den küçükler kesir (0.15 = %15) kabul edilir.
        /// </summary>
        public static decimal CalculateDiscount(decimal subTotal, decimal discountRate)
        {
            if (subTotal <= 0m || discountRate <= 0m)
            {
                return 0m;
            }

            var fraction = discountRate >= 1.0m ? discountRate / 100.0m : discountRate;
            return Math.Round(subTotal * fraction, 2);
        }

        public static decimal CalculateNetTotal(decimal subTotal, decimal discountAmount)
        {
            return Math.Max(0m, subTotal - discountAmount);
        }

        public static decimal CalculateAvailableCredit(decimal creditLimit, decimal currentBalance)
        {
            return Math.Max(0m, creditLimit - currentBalance);
        }

        public static DealerCreditValidationResult ValidateCredit(decimal creditLimit, decimal currentBalance, decimal orderTotal)
        {
            var availableCredit = CalculateAvailableCredit(creditLimit, currentBalance);
            return availableCredit >= orderTotal
                ? DealerCreditValidationResult.SufficientCredit
                : DealerCreditValidationResult.InsufficientCredit;
        }
    }
}
