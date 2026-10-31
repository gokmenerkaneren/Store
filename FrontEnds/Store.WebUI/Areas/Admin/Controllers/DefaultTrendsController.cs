using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.CatalogDtos.DefaultTrendDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/DefaultTrends")]
    public class DefaultTrendsController : Controller
    {
        private readonly IDefaultTrendService _defaultTrendService;
        private readonly IProductService _productService; // To list products in modal
        private readonly ICatalogApiService _catalogApiService;

        public DefaultTrendsController(IDefaultTrendService defaultTrendService, IProductService productService, ICatalogApiService catalogApiService)
        {
            _defaultTrendService = defaultTrendService;
            _productService = productService;
            _catalogApiService = catalogApiService;
        }

        [Route("Index")]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var values = await _defaultTrendService.GetAllAsync();
            return View(values);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Guid productId)
        {
            // First check how many trends exist to set OrderIndex
            var existing = await _defaultTrendService.GetAllAsync();
            int maxOrder = 0;
            if (existing.Count > 0) {
                maxOrder = existing.Max(x => x.OrderIndex);
            }

            var createDto = new CreateDefaultTrendDto
            {
                ProductId = productId,
                OrderIndex = maxOrder + 1,
                IsActive = true
            };

            var responseMessage = await _defaultTrendService.CreateAsync(createDto);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "DefaultTrends", new { area = "Admin" });
            }
            return View();
        }

        [Route("DeleteDefaultTrend/{id}")]
        public async Task<IActionResult> DeleteDefaultTrend(Guid id)
        {
            var responseMessage = await _defaultTrendService.DeleteAsync(id);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "DefaultTrends", new { area = "Admin" });
            }
            return View();
        }

        // Action to get products for the Modal Datatable via Ajax
        [HttpGet]
        [Route("GetProductsForModal")]
        public async Task<IActionResult> GetProductsForModal()
        {
            var products = await _productService.GetAllProductsAsync();
            var images = await _catalogApiService.GetAllProductImagesAsync(false);
            
            var activeProducts = products.Where(x => x.IsActive && !x.IsDeleted).Select(p => new {
                id = p.Id,
                productCode = p.ProductCode,
                name = p.Name,
                isActive = p.IsActive,
                imageUrl = images.FirstOrDefault(i => i.ProductId == p.Id && i.IsMain)?.ImageUrl ?? ""
            }).ToList();
            
            return Json(new { data = activeProducts });
        }
    }
}
