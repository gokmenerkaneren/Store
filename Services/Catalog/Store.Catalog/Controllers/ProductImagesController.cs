using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.ProductImageDtos;
using Store.Catalog.Services.ProductImageServices;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _productImageService;

        public ProductImagesController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ProductImageList(bool showDeleted = false)
        {
            var result = await _productImageService.GetAllProductImageAsync(showDeleted);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductImageById(Guid id)
        {
            var result = await _productImageService.GetByIdProductImageAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("GetByProductId/{id}")]
        public async Task<IActionResult> GetByProductId(Guid id)
        {
            var result = await _productImageService.GetByProductIdProductImageAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductImage(CreateProductImageDto createProductImageDto)
        {
            await _productImageService.CreateProductImageAsync(createProductImageDto);
            return Ok("ProductImage Başarılı bir şekilde oluşturuldu !");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductImage(UpdateProductImageDto updateProductImageDto)
        {
            await _productImageService.UpdateProductImageAsync(updateProductImageDto);
            return Ok("ProductImage Başarılı bir şekilde güncellendi !");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductImage(Guid id)
        {
            var existing = await _productImageService.GetByIdProductImageAsync(id);
            if (existing == null)
                return NotFound();

            await _productImageService.DeleteProductImageAsync(id);
            return Ok("ProductImage Başarılı bir şekilde silindi !");
        }

        [HttpPost("SetMainImage")]
        public async Task<IActionResult> SetMainImage([FromQuery] Guid imageId, [FromQuery] Guid productId)
        {
            try
            {
                // 1. ADIM: İlgili ürüne ait tüm resimleri listesiyle çek (GetByProductIdProductImageAsync kullanıyoruz)
                var allImages = await _productImageService.GetByProductIdProductImageAsync(productId);

                if (allImages == null || !allImages.Any())
                {
                    return NotFound("Bu ürüne ait görsel bulunamadı.");
                }

                // 2. ADIM: Bütün resimlerin IsMain özelliğini "false" (Ek Resim) yap ve kaydet.
                foreach (var img in allImages)
                {
                    // Sadece daha önce Vitrin Resmi olanı bulup false yapmak performansı artırır
                    if (img.IsMain)
                    {
                        // DTO Dönüşümü (Elmayı, armut sepetine koyuyoruz)
                        var updateDto = new UpdateProductImageDto
                        {
                            Id = img.Id,
                            ProductId = img.ProductId,
                            ImageUrl = img.ImageUrl,
                            AltText = img.AltText,
                            IsMain = false // Artık Vitrin Resmi değil!
                        };

                        await _productImageService.UpdateProductImageAsync(updateDto);
                    }
                }

                // 3. ADIM: Bize parametre olarak gelen imageId'ye sahip resmi bul ve "Vitrin Resmi" yap.
                var selectedImage = await _productImageService.GetByIdProductImageAsync(imageId);

                if (selectedImage != null)
                {
                    // Yine DTO dönüşümü yapıp IsMain değerini true yapıyoruz
                    var mainUpdateDto = new UpdateProductImageDto
                    {
                        Id = selectedImage.Id,
                        ProductId = selectedImage.ProductId,
                        ImageUrl = selectedImage.ImageUrl,
                        AltText = selectedImage.AltText,
                        IsMain = true // Yeni ana resmimiz bu!
                    };

                    await _productImageService.UpdateProductImageAsync(mainUpdateDto);

                    return Ok(new { success = true, message = "Vitrin Resmi başarıyla güncellendi." });
                }

                return BadRequest("Seçilen görsel bulunamadı.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu tarafında bir hata oluştu: " + ex.Message);
            }
        }

    }
}

