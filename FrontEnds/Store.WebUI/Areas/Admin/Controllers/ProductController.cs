using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;
using Store.DtoLayer.CatalogDtos.BrandDtos;
using Store.DtoLayer.CatalogDtos.CategoryDtos;
using Store.DtoLayer.CatalogDtos.MeasurementDtos;

using Store.DtoLayer.CatalogDtos.ProductDtos;
using Store.DtoLayer.CatalogDtos.ProductImageDtos;
using Store.DtoLayer.CatalogDtos.ProductPriceDtos;
using Store.DtoLayer.CatalogDtos.CurrencyDtos;
using Store.DtoLayer.CatalogDtos.PriceTypeDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ICatalogApiService _catalogApiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ICatalogApiService catalogApiService, IWebHostEnvironment webHostEnvironment)
        {
            _catalogApiService = catalogApiService;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task LoadDropdowns()
        {
            try
            {
                var categories = await _catalogApiService.GetActiveCategoriesAsync();
                ViewBag.Categories = categories?.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList() ?? new List<SelectListItem>();
                ViewBag.CategoryList = categories ?? new List<ResultCategoryDto>();
            }
            catch {
                // EÄŸer API'den Ã§ekerken bir hata oluşursa, liste BOÅ (Count = 0) olarak atanıyor.
                ViewBag.Categories = new List<SelectListItem>(); ViewBag.CategoryList = new List<ResultCategoryDto>(); 
            }

            try
            {
                var brands = await _catalogApiService.GetActiveBrandsAsync();
                ViewBag.Brands = brands?.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList() ?? new List<SelectListItem>();
            }
            catch { ViewBag.Brands = new List<SelectListItem>(); }

            try
            {
                var measurements = await _catalogApiService.GetActiveMeasurementsAsync();
                ViewBag.Measurements = measurements?.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList() ?? new List<SelectListItem>();
            }
            catch { ViewBag.Measurements = new List<SelectListItem>(); }

            try
            {
                var currencies = await _catalogApiService.GetActiveCurrenciesAsync();
                var priceTypes = await _catalogApiService.GetActivePriceTypesAsync();
                ViewBag.Currencies = currencies?.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList() ?? new List<SelectListItem>();
            }
            catch { ViewBag.Currencies = new List<SelectListItem>(); }

            try
            {
                var priceTypes = await _catalogApiService.GetActivePriceTypesAsync();
                ViewBag.PriceTypes = priceTypes?.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList() ?? new List<SelectListItem>();
            }
            catch { ViewBag.PriceTypes = new List<SelectListItem>(); }
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            bool canViewDeleted = Store.WebUI.Helpers.PermissionHelper.CanViewDeletedRecords(User);
            bool actualShowDeleted = canViewDeleted && showDeleted;
            ViewBag.ShowDeleted = actualShowDeleted;

            await LoadDropdowns();
            var categoryList = ViewBag.CategoryList as List<ResultCategoryDto> ?? new List<ResultCategoryDto>();
            try
            {
                ViewBag.AllCategories = await _catalogApiService.GetAllCategoriesAsync(true) ?? new List<ResultCategoryDto>();
                ViewBag.AllBrands = await _catalogApiService.GetAllBrandsAsync(true) ?? new List<ResultBrandDto>();
                ViewBag.AllMeasurements = await _catalogApiService.GetAllMeasurementsAsync(true) ?? new List<ResultMeasurementDto>();
                ViewBag.AllPriceTypes = await _catalogApiService.GetAllPriceTypesAsync(true) ?? new List<ResultPriceTypeDto>();
                ViewBag.AllCurrencies = await _catalogApiService.GetAllCurrenciesAsync(true) ?? new List<ResultCurrencyDto>();
            }
            catch
            {
                ViewBag.AllCategories = new List<ResultCategoryDto>();
                ViewBag.AllBrands = new List<ResultBrandDto>();
                ViewBag.AllMeasurements = new List<ResultMeasurementDto>();
                ViewBag.AllPriceTypes = new List<ResultPriceTypeDto>();
                ViewBag.AllCurrencies = new List<ResultCurrencyDto>();
            }

            try
            {
                var allImages = await _catalogApiService.GetAllProductImagesAsync(actualShowDeleted);
                var mainImages = allImages.Where(x => x.IsMain).ToDictionary(x => x.ProductId, x => x.ImageUrl);
                ViewBag.MainImages = mainImages;
            }
            catch { ViewBag.MainImages = new Dictionary<Guid, string>(); }

            try
            {
                var values = await _catalogApiService.GetAllProductsAsync(actualShowDeleted);
                if (values != null)
                {
                    values = values
                        .OrderBy(p => p.IsDeleted ? 2 : (p.IsActive ? 0 : 1))
                        .ThenBy(p => {
                            var category = categoryList.FirstOrDefault(c => c.Id == p.CategoryId);
                            return category?.Name ?? "";
                        })
                        .ThenBy(p => p.ProductCode ?? "")
                        .ThenBy(p => p.Name ?? "")
                        .ToList();
                }
                return View(values ?? new List<ResultProductDto>());
            }
            catch
            {
                return View(new List<ResultProductDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            return RedirectToAction("CreateOrUpdateProduct");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            try
            {
                var response = await _catalogApiService.CreateProductAsync(createProductDto);
                if (response.IsSuccessStatusCode)
                {
                    var rawContent = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ResultProductDto>(rawContent);
                    return Json(new { success = true, id = product.Id });
                }
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateProduct(Guid? id = null)
        {
            await LoadDropdowns();

            try { ViewBag.AllPriceTypes = await _catalogApiService.GetAllPriceTypesAsync(true); } catch { ViewBag.AllPriceTypes = new List<ResultPriceTypeDto>(); }
            try { ViewBag.AllCurrencies = await _catalogApiService.GetAllCurrenciesAsync(true); } catch { ViewBag.AllCurrencies = new List<ResultCurrencyDto>(); }

            if (!id.HasValue || id.Value == Guid.Empty)
            {
                ViewBag.IsUpdate = false;
                ViewBag.ProductId = Guid.Empty;
                return View(new UpdateProductDto { IsActive = true });
            }

            ViewBag.IsUpdate = true;
            ViewBag.ProductId = id.Value;

            try
            {
                var product = await _catalogApiService.GetProductByIdAsync(id.Value);
                var updateDto = new UpdateProductDto
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode,
                    Barcode = product.Barcode,
                    Name = product.Name,
                    ShortDescription = product.ShortDescription,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    BrandId = product.BrandId,
                    MeasurementId = product.MeasurementId,
                    TaxRate = product.TaxRate,
                    IsPublished = product.IsPublished,


                    Slug = product.Slug,
                    MetaTitle = product.MetaTitle,
                    MetaDescription = product.MetaDescription,
                    IsActive = product.IsActive,
                    Notes = product.Notes,
                    Weight = product.Weight,
                    Desi = product.Desi,
                    Width = product.Width,
                    Height = product.Height,
                    Depth = product.Depth,
                    Tags = product.Tags ?? new List<string>(),
                    Attributes = product.Attributes ?? new List<ProductAttributeDto>(),
                    Variants = product.Variants ?? new List<ProductVariantDto>()
                };



                try { ViewBag.ProductPrices = await _catalogApiService.GetProductPricesByProductIdAsync(id.Value); }
                catch { ViewBag.ProductPrices = new List<ResultProductPriceDto>(); }

                try { ViewBag.ProductImages = await _catalogApiService.GetProductImagesByProductIdAsync(id.Value); }
                catch { ViewBag.ProductImages = new List<ResultProductImageDto>(); }

                return View(updateDto);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            try
            {
                var response = await _catalogApiService.UpdateProductAsync(updateProductDto);
                if (response.IsSuccessStatusCode) return Json(new { success = true });
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateProductPrice(CreateProductPriceDto createProductPriceDto)
        {
            try
            {
                createProductPriceDto.InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001");
                var responseMessage = await _catalogApiService.CreateProductPriceAsync(createProductPriceDto);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var updatedPrices = await _catalogApiService.GetProductPricesByProductIdAsync(createProductPriceDto.ProductId);
                    return PartialView("_ProductPriceListPartial", updatedPrices);
                }
                return BadRequest("Fiyat eklenemedi.");
            }
            catch
            {
                return BadRequest("Sunucu hatası.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductImage(CreateProductImageDto createProductImageDto)
        {
            try
            {
                createProductImageDto.InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001");
                var responseMessage = await _catalogApiService.CreateProductImageAsync(createProductImageDto);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var updatedImages = await _catalogApiService.GetProductImagesByProductIdAsync(createProductImageDto.ProductId);
                    return PartialView("_ProductImageListPartial", updatedImages);
                }
                return BadRequest("Resim eklenemedi.");
            }
            catch
            {
                return BadRequest("Sunucu hatası.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(Guid id, Guid productId)
        {
            try
            {
                var responseMessage = await _catalogApiService.DeleteProductImageAsync(id);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var updatedImages = await _catalogApiService.GetProductImagesByProductIdAsync(productId);
                    return PartialView("_ProductImageListPartial", updatedImages);
                }
                return BadRequest("Resim silinemedi.");
            }
            catch
            {
                return BadRequest("Sunucu hatası.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetMainProductImage(Guid productId, Guid imageId)
        {
            try
            {
                var responseMessage = await _catalogApiService.SetMainProductImageAsync(productId, imageId);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var updatedImages = await _catalogApiService.GetProductImagesByProductIdAsync(productId);
                    return PartialView("_ProductImageListPartial", updatedImages);
                }
                return BadRequest("Ana resim ayarlanamadı.");
            }
            catch
            {
                return BadRequest("Sunucu hatası.");
            }
        }

        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var responseMessage = await _catalogApiService.DeleteProductAsync(id);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ãœrün silinirken bir hata oluştu: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RestoreProduct(Guid id)
        {
            try
            {
                var responseMessage = await _catalogApiService.RestoreProductAsync(id);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ãœrün geri yüklenirken bir hata oluştu: " + ex.Message;
            }
            return RedirectToAction("Index", new { showDeleted = true });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGeneral(UpdateProductDto updateProductDto)
        {
            try
            {
                var product = await _catalogApiService.GetProductByIdAsync(updateProductDto.Id);
                
                updateProductDto.BrandId = product.BrandId;
                updateProductDto.Description = product.Description;
                updateProductDto.Notes = product.Notes;
                updateProductDto.Width = product.Width;
                updateProductDto.Height = product.Height;
                updateProductDto.Depth = product.Depth;
                updateProductDto.Tags = product.Tags ?? new List<string>();
                updateProductDto.Attributes = product.Attributes ?? new List<ProductAttributeDto>();
                updateProductDto.Variants = product.Variants ?? new List<ProductVariantDto>();

                var response = await _catalogApiService.UpdateProductAsync(updateProductDto);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDetail(Guid productId, Guid? brandId, string tag, string description, string notes, double? width, double? height, double? depth)
        {
            try
            {
                var product = await _catalogApiService.GetProductByIdAsync(productId);
                var updateDto = new UpdateProductDto
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode,
                    Barcode = product.Barcode,
                    Name = product.Name,
                    ShortDescription = product.ShortDescription,
                    CategoryId = product.CategoryId,
                    MeasurementId = product.MeasurementId,
                    TaxRate = product.TaxRate,
                    Weight = product.Weight,
                    Desi = product.Desi,
                    IsActive = product.IsActive,
                    IsPublished = product.IsPublished,


                    Slug = product.Slug,
                    MetaTitle = product.MetaTitle,
                    MetaDescription = product.MetaDescription,
                    Attributes = product.Attributes ?? new List<ProductAttributeDto>(),
                    Variants = product.Variants ?? new List<ProductVariantDto>(),
                    
                    BrandId = brandId,
                    Description = description,
                    Notes = notes,
                    Width = width,
                    Height = height,
                    Depth = depth,
                    Tags = ParseTags(tag)
                };

                var response = await _catalogApiService.UpdateProductAsync(updateDto);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(response);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPrice(CreateProductPriceDto createProductPriceDto)
        {
            try
            {
                createProductPriceDto.InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001");
                var responseMessage = await _catalogApiService.CreateProductPriceAsync(createProductPriceDto);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

                [HttpPost]
        public async Task<IActionResult> DeletePrice(Guid id)
        {
            try
            {
                var response = await _catalogApiService.DeleteProductPriceAsync(id);
                if (response.IsSuccessStatusCode) return Json(new { success = true });
                return Json(new { success = false, message = "Fiyat silinemedi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrice(UpdateProductPriceDto updateProductPriceDto)
        {
            try
            {
                updateProductPriceDto.UpdateUserId = Guid.Parse("00000001-0001-0001-0001-000000000001");
                updateProductPriceDto.UpdateDate = DateTime.Now;
                var response = await _catalogApiService.UpdateProductPriceAsync(updateProductPriceDto);
                if (response.IsSuccessStatusCode) return Json(new { success = true });
                return Json(new { success = false, message = "Fiyat gï¿½ncellenemedi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPricesForProduct(Guid productId)
        {
            try
            {
                var prices = await _catalogApiService.GetProductPricesByProductIdAsync(productId);
                var currencies = await _catalogApiService.GetAllCurrenciesAsync(true);
                var priceTypes = await _catalogApiService.GetAllPriceTypesAsync(true);
                
                var result = prices.Select(p => {
                    var pt = priceTypes?.FirstOrDefault(x => x.Id == p.PriceTypeId);
                    var ptName = pt?.Name ?? "-";
                    if (pt != null) {
                        if (pt.IsDeleted) ptName = $"<strong class=\"text-danger\">(SÄ°LÄ°NMÄ°Å)</strong> - {ptName}";
                        else if (!pt.IsActive) ptName = $"<strong class=\"text-warning\">(PASÄ°F)</strong> - {ptName}";
                    }

                    var curr = currencies?.FirstOrDefault(c => c.Id == p.CurrencyId);
                    var currName = curr?.Name ?? "-";
                    if (curr != null) {
                        if (curr.IsDeleted) currName = $"<strong class=\"text-danger\">(SÄ°LÄ°NMÄ°Å)</strong> - {currName}";
                        else if (!curr.IsActive) currName = $"<strong class=\"text-warning\">(PASÄ°F)</strong> - {currName}";
                    }

                    return new { 
                        id = p.Id, 
                        isActive = p.IsActive, 
                        priceTypeName = ptName,
                        amount = p.Amount.ToString("N2"),
                        currencyName = currName,
                        startDate = p.ValidFrom?.ToString("dd.MM.yyyy - HH:mm:ss") ?? "-",
                        endDate = p.ValidTo?.ToString("dd.MM.yyyy - HH:mm:ss") ?? "-"
                    };
                }).ToList();

                return Json(result);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(Guid productId, IFormFile file, string altText, bool isMain)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var path = Path.Combine(directoryPath, fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var imageUrl = "/images/products/" + fileName;

                    var parsedAltText = string.Join(", ", ParseTags(altText));
                    var createProductImageDto = new CreateProductImageDto
                    {
                        ProductId = productId,
                        ImageUrl = imageUrl,
                        AltText = parsedAltText,
                        IsMain = isMain,
                        InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                        IsActive = true,
                        DisplayOrder = 0
                    };

                    var responseMessage = await _catalogApiService.CreateProductImageAsync(createProductImageDto);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, url = imageUrl });
                    }
                    var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
                    return Json(new { success = false, message = err });
                }
                return Json(new { success = false, message = "Lütfen bir dosya seÃ§in." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            try
            {
                var responseMessage = await _catalogApiService.DeleteProductImageAsync(id);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetMainImage(Guid productId, Guid imageId)
        {
            try
            {
                var responseMessage = await _catalogApiService.SetMainProductImageAsync(productId, imageId);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                var err = await Store.WebUI.Helpers.ErrorHelper.GetErrorMessageAsync(responseMessage);
                return Json(new { success = false, message = err });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private List<string> ParseTags(string tagInput)
        {
            if (string.IsNullOrWhiteSpace(tagInput))
                return new List<string>();

            tagInput = tagInput.Trim();
            if (tagInput.StartsWith("["))
            {
                try
                {
                    var items = JsonConvert.DeserializeObject<List<TagifyItem>>(tagInput);
                    if (items != null)
                    {
                        return items.Select(x => x.Value.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
                    }
                }
                catch { }
            }

            return tagInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(t => t.Trim())
                           .Where(t => !string.IsNullOrEmpty(t))
                           .ToList();
        }

        private class TagifyItem
        {
            public string Value { get; set; } = string.Empty;
        }
    }
}





