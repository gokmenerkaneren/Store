using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.DefaultSlider1Dtos;
using Store.Catalog.Services.DefaultSlider1Services;
using System;
using System.Threading.Tasks;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultSlider1Controller : ControllerBase
    {
        private readonly IDefaultSlider1Service _service;

        public DefaultSlider1Controller(IDefaultSlider1Service service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(bool showDeleted = false)
        {
            var result = await _service.GetAllAsync(showDeleted);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("datatables")]
        public async Task<IActionResult> GetForDataTables([FromBody] Store.Shared.Dtos.DataTablesRequest request, [FromQuery] bool showDeleted = false)
        {
            var result = await _service.GetForDataTablesAsync(request, showDeleted);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDefaultSlider1Dto dto)
        {
            try
            {
                await _service.CreateAsync(dto);
                return Ok("Slider basariyla olusturuldu.");
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDefaultSlider1Dto dto)
        {
            try
            {
                await _service.UpdateAsync(dto);
                return Ok("Slider basariyla guncellendi.");
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok("Slider basariyla silindi.");
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await _service.RestoreAsync(id);
                return Ok("Slider basariyla geri yuklendi.");
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
