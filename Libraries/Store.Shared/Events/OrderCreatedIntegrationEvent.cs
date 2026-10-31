namespace Store.Shared.Events
{
    /// <summary>
    /// Bir sipariş oluşturulduğunda Order servisi tarafından yayınlanan entegrasyon olayı.
    /// </summary>
    public record OrderCreatedIntegrationEvent(
        Guid OrderId,
        Guid UserId,
        decimal TotalPrice,
        DateTime OrderDate,
        List<OrderItemMessageDto> Items);

    /// <summary>
    /// Entegrasyon olayı içinde taşınan sipariş kalemi.
    /// </summary>
    public record OrderItemMessageDto(
        Guid ProductId,
        int Quantity,
        decimal UnitPrice);
}
