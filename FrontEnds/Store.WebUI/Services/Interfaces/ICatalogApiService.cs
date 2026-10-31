using Refit;
using Store.DtoLayer.CatalogDtos.BrandDtos;
using Store.DtoLayer.CatalogDtos.CategoryDtos;
using Store.DtoLayer.CatalogDtos.MeasurementDtos;

using Store.DtoLayer.CatalogDtos.ProductDtos;
using Store.DtoLayer.CatalogDtos.ProductImageDtos;
using Store.DtoLayer.CatalogDtos.ProductPriceDtos;
using Store.DtoLayer.CatalogDtos.CurrencyDtos;
using Store.DtoLayer.CatalogDtos.PriceTypeDtos;
using Store.DtoLayer.CatalogDtos.ColorDtos;   // Renk lookup
using Store.DtoLayer.CatalogDtos.SizeDtos;    // Beden lookup
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface ICatalogApiService
    {
        // Products
        [Get("/api/Products?showDeleted={showDeleted}")]
        Task<List<ResultProductDto>> GetAllProductsAsync(bool showDeleted);

        [Get("/api/Products/{id}")]
        Task<ResultProductDto> GetProductByIdAsync(Guid id);

        [Post("/api/Products")]
        Task<HttpResponseMessage> CreateProductAsync([Body] CreateProductDto createProductDto);

        [Put("/api/Products")]
        Task<HttpResponseMessage> UpdateProductAsync([Body] UpdateProductDto updateProductDto);

        [Delete("/api/Products/{id}")]
        Task<HttpResponseMessage> DeleteProductAsync(Guid id);

        [Post("/api/Products/Restore/{id}")]
        Task<HttpResponseMessage> RestoreProductAsync(Guid id);

        // Product Prices
        [Get("/api/ProductPrices/GetByProductId/{id}")]
        Task<List<ResultProductPriceDto>> GetProductPricesByProductIdAsync(Guid id);

        [Post("/api/ProductPrices")]
        Task<HttpResponseMessage> CreateProductPriceAsync([Body] CreateProductPriceDto createProductPriceDto);
        [Put("/api/ProductPrices")]
        Task<HttpResponseMessage> UpdateProductPriceAsync([Body] UpdateProductPriceDto updateProductPriceDto);
        [Delete("/api/ProductPrices/{id}")]
        Task<HttpResponseMessage> DeleteProductPriceAsync(Guid id);

        // Product Images
        [Get("/api/ProductImages/GetByProductId/{id}")]
        Task<List<ResultProductImageDto>> GetProductImagesByProductIdAsync(Guid id);

        [Get("/api/ProductImages?showDeleted={showDeleted}")]
        Task<List<ResultProductImageDto>> GetAllProductImagesAsync(bool showDeleted = false);

        [Post("/api/ProductImages")]
        Task<HttpResponseMessage> CreateProductImageAsync([Body] CreateProductImageDto createProductImageDto);

        [Delete("/api/ProductImages/{id}")]
        Task<HttpResponseMessage> DeleteProductImageAsync(Guid id);

        [Post("/api/ProductImages/SetMainImage?productId={productId}&imageId={imageId}")]
        Task<HttpResponseMessage> SetMainProductImageAsync(Guid productId, Guid imageId);

        // Lookups
        [Get("/api/categories?showDeleted=false")]
        Task<List<ResultCategoryDto>> GetActiveCategoriesAsync();

        [Get("/api/categories?showDeleted={showDeleted}")]
        Task<List<ResultCategoryDto>> GetAllCategoriesAsync(bool showDeleted);

        [Get("/api/brands?showDeleted=false")]
        Task<List<ResultBrandDto>> GetActiveBrandsAsync();

        [Get("/api/brands?showDeleted={showDeleted}")]
        Task<List<ResultBrandDto>> GetAllBrandsAsync(bool showDeleted);

        [Get("/api/brands/{id}")]
        Task<ResultBrandDto> GetBrandByIdAsync(Guid id);

        [Get("/api/measurements?showDeleted=false")]
        Task<List<ResultMeasurementDto>> GetActiveMeasurementsAsync();

        [Get("/api/measurements?showDeleted={showDeleted}")]
        Task<List<ResultMeasurementDto>> GetAllMeasurementsAsync(bool showDeleted);

        [Get("/api/currencies?showDeleted=false")]
        Task<List<ResultCurrencyDto>> GetActiveCurrenciesAsync();

        [Get("/api/currencies?showDeleted={showDeleted}")]
        Task<List<ResultCurrencyDto>> GetAllCurrenciesAsync(bool showDeleted);

        [Get("/api/PriceTypes?showDeleted=false")]
        Task<List<ResultPriceTypeDto>> GetActivePriceTypesAsync();

        [Get("/api/PriceTypes?showDeleted={showDeleted}")]
        Task<List<ResultPriceTypeDto>> GetAllPriceTypesAsync(bool showDeleted);

        // Colors - Renk lookup (dropdown ve sidebar için)
        [Get("/api/colors?showDeleted=false")]
        Task<List<ResultColorDto>> GetActiveColorsAsync();

        [Get("/api/colors?showDeleted={showDeleted}")]
        Task<List<ResultColorDto>> GetAllColorsAsync(bool showDeleted);

        // Sizes - Beden lookup (dropdown ve sidebar için)
        [Get("/api/sizes?showDeleted=false")]
        Task<List<ResultSizeDto>> GetActiveSizesAsync();

        [Get("/api/sizes?showDeleted={showDeleted}")]
        Task<List<ResultSizeDto>> GetAllSizesAsync(bool showDeleted);
    }
}


