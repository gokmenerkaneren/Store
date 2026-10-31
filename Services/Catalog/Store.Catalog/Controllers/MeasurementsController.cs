using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.MeasurementDtos;
using Store.Catalog.Services.MeasurementServices;
using System;
using System.Threading.Tasks;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        private readonly IMeasurementService _service;

        public MeasurementsController(IMeasurementService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> MeasurementList(bool showDeleted = false)
        {
            var result = await _service.GetAllMeasurementAsync(showDeleted);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMeasurementById(Guid id)
        {
            var result = await _service.GetByIdMeasurementAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeasurement(CreateMeasurementDto dto)
        {
            try
            {
                await _service.CreateMeasurementAsync(dto);
                return Ok("Kayıt başarılı bir şekilde oluşturuldu !");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMeasurement(UpdateMeasurementDto dto)
        {
            try
            {
                await _service.UpdateMeasurementAsync(dto);
                return Ok("Kayıt başarılı bir şekilde güncellendi !");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeasurement(Guid id)
        {
            await _service.DeleteMeasurementAsync(id);
            return Ok("Kayıt başarılı bir şekilde silindi !");
        }

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreMeasurement(Guid id)
        {
            await _service.RestoreMeasurementAsync(id);
            return Ok("Kayıt başarılı bir şekilde geri yüklendi !");
        }
    }
}
