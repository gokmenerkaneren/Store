using MediatR;
using Microsoft.AspNetCore.Authorization;

using Store.Catalog.Commands;
using Store.Catalog.Queries;
using Store.Catalog.Services.ProductServices;
using Store.Catalog.Dtos.ProductDtos;
using Microsoft.AspNetCore.Mvc;

using Store.Catalog.Services.ProductServices;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMediator _mediator;

        public ProductsController(IProductService productService, IMediator mediator)
        {
            _productService = productService;
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ProductList(bool showDeleted = false)
        {
            var result = await _mediator.Send(new GetAllProductsQuery(showDeleted));
            return Ok(result);
        }

        [HttpPost("DataTables")]
        public async Task<IActionResult> GetProductDataTables([FromBody] Store.Shared.Dtos.DataTablesRequest request, [FromQuery] bool showDeleted = false)
        {
            var result = await _productService.GetDataTablesAsync(request, showDeleted);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _productService.GetByIdProductAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("CheckStock/{id}")]
        public async Task<IActionResult> CheckStock(Guid id, [FromQuery] int requestedQuantity = 1)
        {
            var result = await _productService.GetByIdProductAsync(id);
            if (result == null)
                return NotFound(new { message = "Ürün bulunamadı." });

            int totalStock = result.Variants != null && result.Variants.Any()
                ? result.Variants.Sum(v => v.StockQuantity)
                : 0;

            bool isAvailable = totalStock >= requestedQuantity;
            return Ok(new
            {
                productId = id,
                requestedQuantity,
                availableStock = totalStock,
                isAvailable
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            try
            {
                var createdProduct = await _mediator.Send(new CreateProductCommand(createProductDto));
                return Ok(createdProduct);
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            try
            {
                await _mediator.Send(new UpdateProductCommand(updateProductDto));
                return Ok("Product Başarılı bir şekilde güncellendi !");
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var existing = await _productService.GetByIdProductAsync(id);
            if (existing == null)
                return NotFound();

            await _productService.DeleteProductAsync(id);
            return Ok("Product Başarılı bir şekilde silindi !");
        }
        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreProduct(Guid id)
        {
            await _productService.RestoreProductAsync(id);
            return Ok("Product Başarılı bir şekilde geri yüklendi !");
        }
    }
}

