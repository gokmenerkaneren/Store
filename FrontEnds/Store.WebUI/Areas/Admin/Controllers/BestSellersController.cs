using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.CatalogDtos.BestSellerDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/BestSellers")]
    public class BestSellersController : Controller
    {
        private readonly IBestSellerApiService _bestSellerApiService;
        private readonly IProductService _productService; // To list products in modal
        private readonly ICatalogApiService _catalogApiService;

        public BestSellersController(IBestSellerApiService bestSellerApiService, IProductService productService, ICatalogApiService catalogApiService)
        {
            _bestSellerApiService = bestSellerApiService;
            _productService = productService;
            _catalogApiService = catalogApiService;
        }

        [Route("Index")]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var values = await _bestSellerApiService.GetAllAsync();
            return View(values);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Guid productId)
        {
            // First check how many best sellers exist to set OrderIndex
            var existing = await _bestSellerApiService.GetAllAsync();
            int maxOrder = 0;
            if (existing.Count > 0) {
                maxOrder = existing.Max(x => x.OrderIndex);
            }

            var createDto = new CreateBestSellerDto
            {
                ProductId = productId,
                OrderIndex = maxOrder + 1
            };

            var responseMessage = await _bestSellerApiService.CreateAsync(createDto);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "BestSellers", new { area = "Admin" });
            }
            return View();
        }

        [Route("DeleteBestSeller/{id}")]
        public async Task<IActionResult> DeleteBestSeller(Guid id)
        {
            var responseMessage = await _bestSellerApiService.DeleteAsync(id);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "BestSellers", new { area = "Admin" });
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
