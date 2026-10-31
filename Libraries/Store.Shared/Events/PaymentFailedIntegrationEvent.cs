namespace Store.Shared.Events
{
    /// <summary>
    /// Sipariş ödemesi reddedildiğinde Payment servisi tarafından yayınlanır.
    /// Catalog stoğu, B2B bayi bakiyesini geri alır; Order siparişi iptal eder.
    /// </summary>
    public record PaymentFailedIntegrationEvent(
        Guid OrderId,
        Guid UserId,
        decimal TotalPrice,
        string Reason,
        DateTime FailedDate,
        List<OrderItemMessageDto> Items);
}
