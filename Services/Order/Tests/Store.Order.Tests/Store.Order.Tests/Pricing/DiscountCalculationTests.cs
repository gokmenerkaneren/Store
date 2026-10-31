using FluentAssertions;
using Store.Shared.Pricing;
using System;
using Xunit;

namespace Store.Order.Tests.Pricing
{
    public class DiscountCalculationTests
    {
        private static readonly DateTime Now = new DateTime(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void CalculateDiscount_Should_Apply_Percentage_Rate_Correctly()
        {
            var discount = CouponPricing.CalculateDiscount(500m, 20);
            var net = CouponPricing.CalculateNetTotal(500m, discount);

            discount.Should().Be(100m);
            net.Should().Be(400m);
        }

        [Fact]
        public void ValidateCoupon_Should_Reject_When_Expired()
        {
            var result = CouponPricing.Validate(true, true, Now.AddSeconds(-1), Now);

            result.Should().Be(CouponValidationResult.Expired);
        }

        [Fact]
        public void ValidateCoupon_Should_Reject_When_Not_Found_Or_Inactive()
        {
            var notFound = CouponPricing.Validate(false, false, default, Now);
            var inactive = CouponPricing.Validate(true, false, Now.AddDays(1), Now);
            var active = CouponPricing.Validate(true, true, Now.AddDays(1), Now);

            notFound.Should().Be(CouponValidationResult.NotFoundOrInactive);
            inactive.Should().Be(CouponValidationResult.NotFoundOrInactive);
            active.Should().Be(CouponValidationResult.Valid);
        }

        [Fact]
        public void CalculateDiscount_Should_Never_Result_In_Negative_Total()
        {
            var discount = CouponPricing.CalculateDiscount(200m, 150);
            var net = CouponPricing.CalculateNetTotal(200m, discount);

            discount.Should().Be(300m);
            net.Should().Be(0m);
        }
    }
}
