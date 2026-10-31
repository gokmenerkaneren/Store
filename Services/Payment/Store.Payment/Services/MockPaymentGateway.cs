namespace Store.Payment.Services
{
    /// <summary>
    /// Gerçek sağlayıcı (iyzico/PayTR) entegre edilene kadar kullanılan sahte ödeme altyapısı.
    /// 0000 ile biten kartlar reddedilir, diğer tüm istekler başarılı sayılır.
    /// </summary>
    public class MockPaymentGateway : IPaymentGateway
    {
        public Task<PaymentProcessResult> ProcessPaymentAsync(PaymentProcessRequest request, CancellationToken cancellationToken = default)
        {
            var result = request.CardNumber != null && request.CardNumber.EndsWith("0000")
                ? new PaymentProcessResult(false, null, "Kart provizyon hatası: Yetersiz bakiye.")
                : new PaymentProcessResult(true, Guid.NewGuid().ToString(), null);

            return Task.FromResult(result);
        }
    }
}
