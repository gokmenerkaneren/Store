namespace Store.DtoLayer.CatalogDtos.InventoryDtos
{
    public class ResultInventoryDto
    {
        public Guid Id { get; set; }
        public int AvailableQuantity { get; set; }
        public int RentedQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AlertMinimumStockLevel { get; set; }
        public string? ShelfLocation { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }

    }
}
