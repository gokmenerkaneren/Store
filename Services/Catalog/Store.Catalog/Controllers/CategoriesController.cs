using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.CategoryDtos;
using Store.Catalog.Services.CategoryServices;

namespace Store.Catalog.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CategoryList(bool showDeleted = false)
        {
            var result = await _categoryService.GetAllCategoryAsync(showDeleted);
            return Ok(result);
        }

        [HttpPost("datatables")]
        public async Task<IActionResult> GetCategoryDataTables([FromBody] Store.Shared.Dtos.DataTablesRequest request, [FromQuery] bool showDeleted = false)
        {
            var result = await _categoryService.GetCategoriesForDataTablesAsync(request, showDeleted);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _categoryService.GetByIdCategoryAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddorEdit(CreateCategoryDto createCategoryDto)
        {
            try
            {
                await _categoryService.AddorEditAsync(createCategoryDto);
                return Ok("Category Başarılı bir şekilde oluşturuldu !");
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                await _categoryService.UpdateCategoryAsync(updateCategoryDto);
                return Ok("Category Başarılı bir şekilde güncellendi !");
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var existing = await _categoryService.GetByIdCategoryAsync(id);
                if (existing == null)
                    return NotFound();

                await _categoryService.DeleteCategoryAsync(id);
                return Ok("Category Başarılı bir şekilde silindi !");
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Restore/{id}")]
        public async Task<IActionResult> RestoreCategory(Guid id)
        {
            try
            {
                await _categoryService.RestoreCategoryAsync(id);
                return Ok("Category Başarılı bir şekilde geri yüklendi !");
            }
            catch (Store.Catalog.Exceptions.BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}

