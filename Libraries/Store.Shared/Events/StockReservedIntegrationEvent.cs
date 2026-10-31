namespace Store.Shared.Events
{
    /// <summary>
    /// Siparişin tüm kalemleri için stok düşümü başarılı olduğunda Catalog servisi tarafından yayınlanır.
    /// </summary>
    public record StockReservedIntegrationEvent(
        Guid OrderId,
        Guid UserId,
        decimal TotalPrice,
        DateTime ReservedDate,
        List<OrderItemMessageDto> Items);
}
