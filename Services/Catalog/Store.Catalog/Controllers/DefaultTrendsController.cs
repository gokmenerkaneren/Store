using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.DefaultTrendDtos;
using Store.Catalog.Services.DefaultTrendServices;
using System;
using System.Threading.Tasks;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultTrendsController : ControllerBase
    {
        private readonly IDefaultTrendService _defaultTrendService;

        public DefaultTrendsController(IDefaultTrendService defaultTrendService)
        {
            _defaultTrendService = defaultTrendService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> DefaultTrendList()
        {
            var values = await _defaultTrendService.GetAllDefaultTrendAsync();
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDefaultTrendById(Guid id)
        {
            var values = await _defaultTrendService.GetByIdDefaultTrendAsync(id);
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDefaultTrend(CreateDefaultTrendDto createDefaultTrendDto)
        {
            await _defaultTrendService.CreateDefaultTrendAsync(createDefaultTrendDto);
            return Ok("Trend başarıyla eklendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDefaultTrend(Guid id)
        {
            await _defaultTrendService.DeleteDefaultTrendAsync(id);
            return Ok("Trend başarıyla silindi");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDefaultTrend(UpdateDefaultTrendDto updateDefaultTrendDto)
        {
            await _defaultTrendService.UpdateDefaultTrendAsync(updateDefaultTrendDto);
            return Ok("Trend başarıyla güncellendi");
        }
    }
}