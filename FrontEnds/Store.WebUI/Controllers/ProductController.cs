using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Services.Interfaces;
using Store.WebUI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICatalogApiService _catalogApiService;

        public ProductController(ICatalogApiService catalogApiService)
        {
            _catalogApiService = catalogApiService;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            try
            {
                var product = await _catalogApiService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var allImages = await _catalogApiService.GetProductImagesByProductIdAsync(id);
                var mainImage = allImages != null ? allImages.FirstOrDefault(x => x.IsMain)?.ImageUrl : null;
                mainImage ??= "/images/no-image.jpg";

                var imageUrls = allImages?.Select(x => x.ImageUrl).ToList() ?? new List<string>();
                if (!imageUrls.Any())
                {
                    imageUrls.Add(mainImage);
                }

                decimal oldPrice = 0;
                decimal retailPrice = product.Price;

                try
                {
                    var allPrices = await _catalogApiService.GetProductPricesByProductIdAsync(id);
                    if (allPrices != null && allPrices.Any())
                    {
                        var activePrice = allPrices.FirstOrDefault();
                        if (activePrice != null)
                        {
                            oldPrice = activePrice.Amount;
                            retailPrice = activePrice.DiscountedAmount ?? activePrice.Amount;
                        }
                    }
                }
                catch
                {
                    // If price fetch fails, just ignore and use default product price
                }

                var attributes = new List<KeyValuePair<string, string>>();
                if (product.Attributes != null)
                {
                    foreach (var attr in product.Attributes)
                    {
                        attributes.Add(new KeyValuePair<string, string>(attr.Key, attr.Value));
                    }
                }

                string brandName = string.Empty;
                if (product.BrandId.HasValue && product.BrandId.Value != Guid.Empty)
                {
                    try
                    {
                        var brand = await _catalogApiService.GetBrandByIdAsync(product.BrandId.Value);
                        brandName = brand?.Name ?? string.Empty;
                    }
                    catch
                    {
                        // Marka bilgisi getirilemezse boş geçilir
                    }
                }

                var model = new ProductDetailViewModel
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode ?? "",
                    Name = product.Name ?? "Bilinmeyen Ürün",
                    Description = product.Description,
                    MainImageUrl = mainImage,
                    ImageUrls = imageUrls,
                    Height = product.Height,
                    Width = product.Width,
                    Depth = product.Depth,
                    OldPrice = oldPrice,
                    RetailPrice = retailPrice,
                    Slug = product.Slug ?? "",
                    Attributes = attributes,
                    BrandId = product.BrandId,
                    BrandName = brandName,
                    Notes = product.Notes
                };

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Default");
            }
        }

        public async Task<IActionResult> QuickView(Guid id)
        {
            try
            {
                var product = await _catalogApiService.GetProductByIdAsync(id);
                var allImages = await _catalogApiService.GetProductImagesByProductIdAsync(id);
                
                var mainImage = allImages != null ? allImages.FirstOrDefault(x => x.IsMain)?.ImageUrl : null;
                mainImage ??= "/images/no-image.jpg";

                decimal oldPrice = 0;
                decimal retailPrice = product?.Price ?? 0;

                try
                {
                    var allPrices = await _catalogApiService.GetProductPricesByProductIdAsync(id);
                    if (allPrices != null && allPrices.Any())
                    {
                        var activePrice = allPrices.FirstOrDefault();
                        if (activePrice != null)
                        {
                            oldPrice = activePrice.Amount;
                            retailPrice = activePrice.DiscountedAmount ?? activePrice.Amount;
                        }
                    }
                }
                catch
                {
                    // If price fetch fails, just ignore and use 0
                }

                var attributes = new List<KeyValuePair<string, string>>();
                if (product != null && product.Attributes != null)
                {
                    foreach (var attr in product.Attributes)
                    {
                        attributes.Add(new KeyValuePair<string, string>(attr.Key, attr.Value));
                    }
                }

                var modelItem = new QuickViewViewModel
                {
                    Id = product?.Id ?? Guid.Empty,
                    ProductCode = product?.ProductCode ?? "",
                    Name = product?.Name ?? "Bilinmeyen Ürün",
                    Description = product?.Description, // or ShortDescription based on UI
                    MainImageUrl = mainImage,
                    Height = product?.Height,
                    Width = product?.Width,
                    Depth = product?.Depth,
                    OldPrice = oldPrice,
                    RetailPrice = retailPrice,
                    Slug = product?.Slug ?? "",
                    Attributes = attributes
                };

                return PartialView("_QuickView", modelItem);
            }
            catch (Exception)
            {
                // Hata durumunda boş bir model dön
                return PartialView("_QuickView", new QuickViewViewModel());
            }
        }
    }
}