using System;

namespace Store.Shared.Pricing
{
    public enum CouponValidationResult
    {
        Valid,
        NotFoundOrInactive,
        Expired
    }

    /// <summary>
    /// Kupon doğrulama ve indirim hesaplama kuralları (saf fonksiyonlar).
    /// </summary>
    public static class CouponPricing
    {
        public static CouponValidationResult Validate(bool exists, bool isActive, DateTime validDate, DateTime utcNow)
        {
            if (!exists || !isActive)
            {
                return CouponValidationResult.NotFoundOrInactive;
            }

            return validDate < utcNow
                ? CouponValidationResult.Expired
                : CouponValidationResult.Valid;
        }

        public static decimal CalculateDiscount(decimal subTotal, int rate)
        {
            return Math.Round(subTotal * (rate / 100.0m), 2);
        }

        public static decimal CalculateNetTotal(decimal subTotal, decimal discountAmount)
        {
            return Math.Max(0, subTotal - discountAmount);
        }
    }
}
