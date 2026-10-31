using FluentAssertions;
using Store.Shared.Pricing;
using Xunit;

namespace Store.Order.Tests.Pricing
{
    public class DealerPricingTests
    {
        [Fact]
        public void CalculateDiscount_Should_Apply_Dealer_DiscountRate_Correctly()
        {
            var discount = DealerPricing.CalculateDiscount(10000m, 15m);
            var net = DealerPricing.CalculateNetTotal(10000m, discount);

            discount.Should().Be(1500m);
            net.Should().Be(8500m);
        }

        [Fact]
        public void CalculateDiscount_Should_Support_Decimal_Fraction_DiscountRate()
        {
            var fromFraction = DealerPricing.CalculateDiscount(10000m, 0.15m);
            var fromPercentage = DealerPricing.CalculateDiscount(10000m, 15.0m);

            fromFraction.Should().Be(fromPercentage);
            fromFraction.Should().Be(1500m);

            // 1% discount test (%1 iskonto testi)
            DealerPricing.CalculateDiscount(10000m, 1.0m).Should().Be(100m);
        }

        [Fact]
        public void ValidateCredit_Should_Allow_Order_When_Available_Credit_Is_Sufficient()
        {
            var result = DealerPricing.ValidateCredit(100000m, 20000m, 50000m);

            result.Should().Be(DealerCreditValidationResult.SufficientCredit);
        }

        [Fact]
        public void ValidateCredit_Should_Reject_Order_When_OrderTotal_Exceeds_Available_Credit()
        {
            var result = DealerPricing.ValidateCredit(50000m, 45000m, 6000m);

            result.Should().Be(DealerCreditValidationResult.InsufficientCredit);
        }

        [Fact]
        public void CalculateAvailableCredit_Should_Never_Return_Negative_When_Balance_Exceeds_Limit()
        {
            var available = DealerPricing.CalculateAvailableCredit(50000m, 60000m);

            available.Should().Be(0m);
        }

        [Fact]
        public void CalculateDiscount_Should_Return_Zero_When_DiscountRate_Or_SubTotal_Is_Zero()
        {
            DealerPricing.CalculateDiscount(10000m, 0m).Should().Be(0m);
            DealerPricing.CalculateDiscount(0m, 15m).Should().Be(0m);
        }
    }
}
