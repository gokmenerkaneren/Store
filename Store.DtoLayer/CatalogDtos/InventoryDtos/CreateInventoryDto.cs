namespace Store.DtoLayer.CatalogDtos.InventoryDtos
{
    public class CreateInventoryDto
    {
        public int AvailableQuantity { get; set; }
        public int RentedQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AlertMinimumStockLevel { get; set; }
        public string? ShelfLocation { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
