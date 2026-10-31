using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Services.PriceTypeServices;
using Store.Catalog.Dtos.PriceTypeDtos;
using System;
using System.Threading.Tasks;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PriceTypesController : ControllerBase
    {
        private readonly IPriceTypeService _priceTypeService;

        public PriceTypesController(IPriceTypeService priceTypeService)
        {
            _priceTypeService = priceTypeService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllPriceTypes([FromQuery] bool showDeleted = false)
        {
            var values = await _priceTypeService.GetAllPriceTypeAsync(showDeleted);
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPriceTypeById(Guid id)
        {
            var value = await _priceTypeService.GetByIdPriceTypeAsync(id);
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePriceType(CreatePriceTypeDto createPriceTypeDto)
        {
            try
            {
                await _priceTypeService.CreatePriceTypeAsync(createPriceTypeDto);
                return Ok("PriceType Başarılı bir şekilde oluşturuldu !");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePriceType(UpdatePriceTypeDto updatePriceTypeDto)
        {
            try
            {
                await _priceTypeService.UpdatePriceTypeAsync(updatePriceTypeDto);
                return Ok("PriceType Başarılı bir şekilde güncellendi !");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePriceType(Guid id)
        {
            await _priceTypeService.DeletePriceTypeAsync(id);
            return Ok("PriceType Başarılı bir şekilde silindi !");
        }

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestorePriceType(Guid id)
        {
            await _priceTypeService.RestorePriceTypeAsync(id);
            return Ok("PriceType Başarılı bir şekilde geri yüklendi !");
        }
    }
}
