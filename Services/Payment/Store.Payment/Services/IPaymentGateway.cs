namespace Store.Payment.Services
{
    public record PaymentProcessRequest(Guid OrderId, Guid UserId, decimal Amount, string? CardNumber = null);

    public record PaymentProcessResult(bool IsSuccess, string? TransactionId, string? ErrorMessage);

    public interface IPaymentGateway
    {
        Task<PaymentProcessResult> ProcessPaymentAsync(PaymentProcessRequest request, CancellationToken cancellationToken = default);
    }
}
