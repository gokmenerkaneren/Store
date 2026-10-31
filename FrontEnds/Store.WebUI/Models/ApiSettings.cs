namespace Store.WebUI.Models
{
    public class ApiSettings
    {
        public string CatalogBaseUrl { get; set; }
        public string DiscountBaseUrl { get; set; }
        public string LocationBaseUrl { get; set; }
        public string OrderBaseUrl { get; set; }
        public string IdentityServerUrl { get; set; }
        public string FavoriteBaseUrl { get; set; }
        public string RentalBaseUrl { get; set; } = string.Empty;
        public string BasketBaseUrl { get; set; } = string.Empty;
        public string B2BBaseUrl { get; set; } = string.Empty;
        public string GatewayUrl { get; set; } = string.Empty;
        public string PaymentBaseUrl { get; set; } = string.Empty;
    }
}

