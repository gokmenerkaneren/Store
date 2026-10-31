using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.SizeDtos;
using Store.Catalog.Services.SizeServices;

namespace Store.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> SizeList(bool showDeleted = false)
        {
            var result = await _sizeService.GetAllSizeAsync(showDeleted);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSizeById(Guid id)
        {
            var result = await _sizeService.GetByIdSizeAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSize(CreateSizeDto createSizeDto)
        {
            await _sizeService.CreateSizeAsync(createSizeDto);
            return Ok("Beden başarılı bir şekilde oluşturuldu!");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSize(UpdateSizeDto updateSizeDto)
        {
            await _sizeService.UpdateSizeAsync(updateSizeDto);
            return Ok("Beden başarılı bir şekilde güncellendi!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSize(Guid id)
        {
            var existing = await _sizeService.GetByIdSizeAsync(id);
            if (existing == null)
                return NotFound();

            await _sizeService.DeleteSizeAsync(id);
            return Ok("Beden başarılı bir şekilde silindi!");
        }

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreSize(Guid id)
        {
            await _sizeService.RestoreSizeAsync(id);
            return Ok("Beden başarılı bir şekilde geri yüklendi!");
        }
    }
}
