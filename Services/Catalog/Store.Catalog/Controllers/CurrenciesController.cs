using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.CurrencyDtos;
using Store.Catalog.Services.CurrencyServices;
using System;
using System.Threading.Tasks;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _service;

        public CurrenciesController(ICurrencyService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CurrencyList(bool showDeleted = false)
        {
            var result = await _service.GetAllCurrencyAsync(showDeleted);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurrencyById(Guid id)
        {
            var result = await _service.GetByIdCurrencyAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCurrency(CreateCurrencyDto dto)
        {
            try
            {
                await _service.CreateCurrencyAsync(dto);
                return Ok("Kayıt başarılı bir şekilde oluşturuldu !");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCurrency(UpdateCurrencyDto dto)
        {
            try
            {
                await _service.UpdateCurrencyAsync(dto);
                return Ok("Kayıt başarılı bir şekilde güncellendi !");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(Guid id)
        {
            await _service.DeleteCurrencyAsync(id);
            return Ok("Kayıt başarılı bir şekilde silindi !");
        }

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreCurrency(Guid id)
        {
            await _service.RestoreCurrencyAsync(id);
            return Ok("Kayıt başarılı bir şekilde geri yüklendi !");
        }
    }
}
