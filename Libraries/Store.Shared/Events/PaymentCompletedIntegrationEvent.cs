namespace Store.Shared.Events
{
    /// <summary>
    /// Sipariş ödemesi başarıyla alındığında Payment servisi tarafından yayınlanır.
    /// </summary>
    public record PaymentCompletedIntegrationEvent(
        Guid OrderId,
        Guid UserId,
        decimal TotalPrice,
        DateTime CompletedDate);
}
