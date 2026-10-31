using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.BestSellerDtos;
using Store.Catalog.Services.BestSellerServices;
using System;
using System.Threading.Tasks;

namespace Store.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestSellersController : ControllerBase
    {
        private readonly IBestSellerService _bestSellerService;

        public BestSellersController(IBestSellerService bestSellerService)
        {
            _bestSellerService = bestSellerService;
        }

        [HttpGet]
        public async Task<IActionResult> BestSellerList()
        {
            var values = await _bestSellerService.GetAllBestSellerAsync();
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBestSeller(CreateBestSellerDto createBestSellerDto)
        {
            await _bestSellerService.CreateBestSellerAsync(createBestSellerDto);
            return Ok("En Çok Satan Ürün başarıyla eklendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBestSeller(Guid id)
        {
            await _bestSellerService.DeleteBestSellerAsync(id);
            return Ok("En Çok Satan Ürün başarıyla silindi");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBestSeller(UpdateBestSellerDto updateBestSellerDto)
        {
            await _bestSellerService.UpdateBestSellerAsync(updateBestSellerDto);
            return Ok("En Çok Satan Ürün başarıyla güncellendi");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBestSellerById(Guid id)
        {
            var values = await _bestSellerService.GetByIdBestSellerAsync(id);
            return Ok(values);
        }
    }
}
