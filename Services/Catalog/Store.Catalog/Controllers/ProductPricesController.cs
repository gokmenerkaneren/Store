using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.ProductPriceDtos;
using Store.Catalog.Services.ProductPriceServices;

namespace Store.Catalog.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductPricesController : ControllerBase
    {
        private readonly IProductPriceService _productPriceService;

        public ProductPricesController(IProductPriceService productPriceService)
        {
            _productPriceService = productPriceService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ProductPriceList(bool showDeleted = false)
        {
            var result = await _productPriceService.GetAllProductPriceAsync(showDeleted);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductPriceById(Guid id)
        {
            var result = await _productPriceService.GetByIdProductPriceAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("GetByProductId/{id}")]
        public async Task<IActionResult> GetByProductId(Guid id, bool showDeleted = false)
        {
            var result = await _productPriceService.GetByProductIdProductPriceAsync(id, showDeleted);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductPrice(CreateProductPriceDto createProductPriceDto)
        {
            await _productPriceService.CreateProductPriceAsync(createProductPriceDto);
            return Ok("ProductPrice Başarılı bir şekilde oluşturuldu !");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductPrice(UpdateProductPriceDto updateProductPriceDto)
        {
            await _productPriceService.UpdateProductPriceAsync(updateProductPriceDto);
            return Ok("ProductPrice Başarılı bir şekilde güncellendi !");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductPrice(Guid id)
        {
            await _productPriceService.DeleteProductPriceAsync(id);
            return Ok("ProductPrice Başarılı bir şekilde silindi !");
        }

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreProductPrice(Guid id)
        {
            await _productPriceService.RestoreProductPriceAsync(id);
            return Ok("ProductPrice Başarılı bir şekilde geri yüklendi !");
        }

        [HttpPost("BulkUpdate")]
        [Authorize(Roles = "Admin")] // MVP Admin yetkisi
        public IActionResult BulkUpdatePrices([FromBody] BulkPriceUpdateDto request)
        {
            // Audit (Denetim) için işlemi başlatan kullanıcının ID'sini DTO'ya ekliyoruz
            if (request.AdminUserId == Guid.Empty)
            {
                // Fallback için örnek bir Guid (İlerde UserContextAccessor ile doldurulur)
                request.AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                
                try
                {
                    // Eğer sisteme entegre bir kimlik doğrulama varsa oradan alınır:
                    // request.AdminUserId = Store.Shared.Context.UserContextAccessor.GetClientId() ?? request.AdminUserId;
                }
                catch { }
            }

            // Hangfire kullanarak işlemi arka plan kuyruğuna atıyoruz
            var jobId = Hangfire.BackgroundJob.Enqueue<Store.Catalog.Services.JobServices.IBulkPriceJobService>(
                job => job.ExecuteBulkPriceUpdateAsync(request));

            return Accepted(new { JobId = jobId, Message = "Toplu fiyat güncelleme işlemi arka plan kuyruğuna alındı." });
        }
    }
}

