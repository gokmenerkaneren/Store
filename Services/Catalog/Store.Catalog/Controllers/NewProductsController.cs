using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.NewProductDtos;
using Store.Catalog.Services.NewProductServices;
using System;
using System.Threading.Tasks;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class NewProductsController : ControllerBase
    {
        private readonly INewProductService _newProductService;

        public NewProductsController(INewProductService newProductService)
        {
            _newProductService = newProductService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> NewProductList()
        {
            var values = await _newProductService.GetAllNewProductAsync();
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewProduct(CreateNewProductDto createNewProductDto)
        {
            await _newProductService.CreateNewProductAsync(createNewProductDto);
            return Ok("Yeni Ürün başarıyla eklendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewProduct(Guid id)
        {
            await _newProductService.DeleteNewProductAsync(id);
            return Ok("Yeni Ürün başarıyla silindi");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNewProduct(UpdateNewProductDto updateNewProductDto)
        {
            await _newProductService.UpdateNewProductAsync(updateNewProductDto);
            return Ok("Yeni Ürün başarıyla güncellendi");
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewProductById(Guid id)
        {
            var values = await _newProductService.GetByIdNewProductAsync(id);
            return Ok(values);
        }
    }
}
