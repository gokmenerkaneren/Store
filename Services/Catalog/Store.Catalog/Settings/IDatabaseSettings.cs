namespace Store.Catalog.Settings
{
    public interface IDatabaseSettings
    {
        public string BrandCollectionName { get; set; }
        public string ColorCollectionName { get; set; }   // Renk lookup koleksiyonu
        public string SizeCollectionName { get; set; }    // Beden lookup koleksiyonu
        public string CategoryCollectionName { get; set; }
        public string CurrencyCollectionName { get; set; }
        public string MeasurementCollectionName { get; set; }
        public string PriceTypeCollectionName { get; set; }
        public string ProductCollectionName { get; set; }
        public string ProductDetailCollectionName { get; set; }
        public string ProductImageCollectionName { get; set; }
        public string ProductPriceCollectionName { get; set; }
        public string DefaultSlider1CollectionName { get; set; }
        public string WholesalePriceCollectionName { get; set; }
        public string RetailPriceCollectionName { get; set; }
        public string DefaultTrendCollectionName { get; set; }
        public string BestSellerCollectionName { get; set; }
        public string NewProductCollectionName { get; set; }
        public string InstagramPostCollectionName { get; set; }
        string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
