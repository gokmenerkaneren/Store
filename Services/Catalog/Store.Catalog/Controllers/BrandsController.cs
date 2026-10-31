using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.BrandDtos;
using Store.Catalog.Services.BrandServices;
using System;
using System.Threading.Tasks;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> BrandList(bool showDeleted = false)
        {
            var result = await _brandService.GetAllBrandAsync(showDeleted);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var result = await _brandService.GetByIdBrandAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand(CreateBrandDto createBrandDto)
        {
            await _brandService.CreateBrandAsync(createBrandDto);
            return Ok("Brand Başarılı bir şekilde oluşturuldu !");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBrand(UpdateBrandDto updateBrandDto)
        {
            await _brandService.UpdateBrandAsync(updateBrandDto);
            return Ok("Brand Başarılı bir şekilde güncellendi !");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            var existing = await _brandService.GetByIdBrandAsync(id);
            if (existing == null)
                return NotFound();

            await _brandService.DeleteBrandAsync(id);
            return Ok("Brand Başarılı bir şekilde silindi !");
        }

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreBrand(Guid id)
        {
            await _brandService.RestoreBrandAsync(id);
            return Ok("Brand Başarılı bir şekilde geri yüklendi !");
        }
    }
}
