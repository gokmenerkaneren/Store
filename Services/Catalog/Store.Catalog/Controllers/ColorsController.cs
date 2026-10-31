using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.ColorDtos;
using Store.Catalog.Services.ColorServices;

namespace Store.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColorsController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public async Task<IActionResult> ColorList(bool showDeleted = false)
        {
            var result = await _colorService.GetAllColorAsync(showDeleted);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetColorById(Guid id)
        {
            var result = await _colorService.GetByIdColorAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateColor(CreateColorDto createColorDto)
        {
            await _colorService.CreateColorAsync(createColorDto);
            return Ok("Renk başarılı bir şekilde oluşturuldu!");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateColor(UpdateColorDto updateColorDto)
        {
            await _colorService.UpdateColorAsync(updateColorDto);
            return Ok("Renk başarılı bir şekilde güncellendi!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(Guid id)
        {
            var existing = await _colorService.GetByIdColorAsync(id);
            if (existing == null)
                return NotFound();

            await _colorService.DeleteColorAsync(id);
            return Ok("Renk başarılı bir şekilde silindi!");
        }

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreColor(Guid id)
        {
            await _colorService.RestoreColorAsync(id);
            return Ok("Renk başarılı bir şekilde geri yüklendi!");
        }
    }
}
