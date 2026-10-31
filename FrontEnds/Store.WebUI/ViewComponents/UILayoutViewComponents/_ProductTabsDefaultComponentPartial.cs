using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Services.Interfaces;
using Store.WebUI.Models;
using Store.DtoLayer.CatalogDtos.ProductDtos;
using Store.DtoLayer.CatalogDtos.ProductImageDtos;
using Store.DtoLayer.CatalogDtos.ProductPriceDtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    // "En Cok Satanlar" ve "Yeni Urunler" bolumlerini
    // tek bir tab yapisi icinde birlestirerek sunan ViewComponent.
    // Her iki listenin verisi ayni anda cekilir; View'da JavaScript
    // ile tab gecisi saglanir. Backend cagrisi sadece bir kez yapilir.
    public class _ProductTabsDefaultComponentPartial : ViewComponent
    {
        private readonly ICatalogApiService _catalogApiService;
        private readonly IBestSellerApiService _bestSellerApiService;
        private readonly INewProductApiService _newProductApiService;

        public _ProductTabsDefaultComponentPartial(
            ICatalogApiService catalogApiService,
            IBestSellerApiService bestSellerApiService,
            INewProductApiService newProductApiService)
        {
            _catalogApiService = catalogApiService;
            _bestSellerApiService = bestSellerApiService;
            _newProductApiService = newProductApiService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Tum urunleri ve gorselleri bir kez cekiyoruz (performans tasarrufu)
            var products = await _catalogApiService.GetAllProductsAsync(false);
            var allImages = await _catalogApiService.GetAllProductImagesAsync(false);

            // Perakende fiyat tipi ID'sini belirliyoruz
            Guid? retailPriceTypeId = null;
            try
            {
                var priceTypes = await _catalogApiService.GetActivePriceTypesAsync();
                var retailPriceType = priceTypes
                    .FirstOrDefault(pt => pt.Name.Contains("Perakende") || pt.Name.Contains("Retail"));
                retailPriceTypeId = retailPriceType?.Id;
            }
            catch { }

            // En Cok Satanlar listesini hazirliyoruz
            var bestSellersRaw = await _bestSellerApiService.GetAllAsync();
            var orderedBestSellers = bestSellersRaw.OrderBy(x => x.OrderIndex).ToList();

            var bestSellerProducts = orderedBestSellers
                .Select(t => products.FirstOrDefault(p => p.Id == t.ProductId && p.IsActive && !p.IsDeleted))
                .Where(p => p != null)
                .ToList();

            var bestSellerList = await BuildProductViewModelList(bestSellerProducts!, allImages, retailPriceTypeId);

            // Yeni Urunler listesini hazirliyoruz
            var newProductsRaw = await _newProductApiService.GetAllAsync();
            var orderedNewProducts = newProductsRaw.OrderBy(x => x.OrderIndex).ToList();

            var newProductItems = orderedNewProducts
                .Select(t => products.FirstOrDefault(p => p.Id == t.ProductId && p.IsActive && !p.IsDeleted))
                .Where(p => p != null)
                .ToList();

            var newProductList = await BuildProductViewModelList(newProductItems!, allImages, retailPriceTypeId);

            // Her iki liste birlesik ViewModel'a yerleştiriliyor
            var viewModel = new ProductTabsViewModel
            {
                BestSellers = bestSellerList,
                NewProducts = newProductList
            };

            return View(viewModel);
        }

        // Verilen ResultProductDto listesini gorsel ve fiyat bilgileriyle
        // ProductListViewModel listesine donusturen yardimci metot.
        private async Task<List<ProductListViewModel>> BuildProductViewModelList(
            List<ResultProductDto> activeProducts,
            List<ResultProductImageDto> allImages,
            Guid? retailPriceTypeId)
        {
            var result = new List<ProductListViewModel>();

            foreach (var product in activeProducts)
            {
                decimal retailPrice = 0;
                try
                {
                    var prices = await _catalogApiService.GetProductPricesByProductIdAsync(product.Id);
                    var retailPriceInfo = prices.FirstOrDefault(p =>
                        retailPriceTypeId.HasValue && p.PriceTypeId == retailPriceTypeId.Value && p.IsActive);

                    retailPrice = retailPriceInfo != null ? retailPriceInfo.Amount : product.Price;
                }
                catch
                {
                    retailPrice = product.Price;
                }

                var productImages = allImages.Where(x => x.ProductId == product.Id).ToList();
                var mainImage = productImages.FirstOrDefault(x => x.IsMain);
                var imageUrl = mainImage?.ImageUrl ?? "/images/no-image.jpg";
                var hoverImage = productImages.FirstOrDefault(x => x.Id != mainImage?.Id)?.ImageUrl ?? imageUrl;

                var modelItem = new ProductListViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    RetailPrice = retailPrice,
                    MainImageUrl = imageUrl,
                    HoverImageUrl = hoverImage
                };

                // Variant bilgileri varsa renk ve beden listelerini dolduruyoruz
                if (product.Variants != null && product.Variants.Any())
                {
                    var colors = product.Variants
                        .Select(v => v.Color)
                        .Where(c => !string.IsNullOrEmpty(c))
                        .Distinct()
                        .ToList();
                    if (colors.Any()) modelItem.Colors = colors!;

                    var sizes = product.Variants
                        .Select(v => v.Size)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Distinct()
                        .ToList();
                    if (sizes.Any()) modelItem.Sizes = sizes!;
                }

                result.Add(modelItem);
            }

            return result;
        }
    }
}
