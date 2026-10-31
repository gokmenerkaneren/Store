namespace Store.Shared.Events
{
    /// <summary>
    /// Yetersiz stok veya eşzamanlılık çakışması nedeniyle stok ayrılamadığında Catalog servisi tarafından yayınlanır.
    /// </summary>
    public record StockReservationFailedIntegrationEvent(
        Guid OrderId,
        Guid UserId,
        decimal TotalPrice,
        string Reason,
        DateTime FailedDate,
        List<OrderItemMessageDto> Items);
}
