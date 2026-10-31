using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Services.Interfaces;
using Store.WebUI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _PromoDefaultComponentPartial : ViewComponent
    {
        private readonly ICatalogApiService _catalogApiService;

        public _PromoDefaultComponentPartial(ICatalogApiService catalogApiService)
        {
            _catalogApiService = catalogApiService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var modelList = new List<ProductListViewModel>();

            var products = await _catalogApiService.GetAllProductsAsync(false);
            var activeProducts = products.Where(x => x.IsActive).OrderByDescending(x => x.InsertDate).Take(8).ToList();
            var allImages = await _catalogApiService.GetAllProductImagesAsync(false);
            
            Guid? retailPriceTypeId = null;
            try
            {
                var priceTypes = await _catalogApiService.GetActivePriceTypesAsync();
                var retailPriceType = priceTypes.FirstOrDefault(pt => pt.Name.Contains("Perakende") || pt.Name.Contains("Retail"));
                retailPriceTypeId = retailPriceType?.Id;
            }
            catch { }

            foreach (var product in activeProducts)
            {
                decimal retailPrice = 0;
                try
                {
                    var prices = await _catalogApiService.GetProductPricesByProductIdAsync(product.Id);
                    var retailPriceInfo = prices.FirstOrDefault(p => retailPriceTypeId.HasValue && p.PriceTypeId == retailPriceTypeId.Value && p.IsActive);
                    if (retailPriceInfo != null)
                    {
                        retailPrice = retailPriceInfo.Amount;
                    }
                    else
                    {
                        retailPrice = product.Price;
                    }
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

                if (product.Variants != null)
                {
                    var colors = product.Variants.Select(v => v.Color).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
                    if (colors.Any())
                    {
                        modelItem.Colors = colors;
                    }

                    var sizes = product.Variants.Select(v => v.Size).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                    if (sizes.Any())
                    {
                        modelItem.Sizes = sizes;
                    }
                }

                modelList.Add(modelItem);
            }

            return View(modelList);
        }
    }
}

