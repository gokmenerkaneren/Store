using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Catalog.Dtos.InstagramPostDtos;
using Store.Catalog.Services.InstagramPostServices;
using System.Threading.Tasks;

namespace Store.Catalog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class InstagramPostsController : ControllerBase
    {
        private readonly IInstagramPostService _instagramPostService;

        public InstagramPostsController(IInstagramPostService instagramPostService)
        {
            _instagramPostService = instagramPostService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> InstagramPostList()
        {
            var values = await _instagramPostService.GetAllInstagramPostsAsync();
            return Ok(values);
        }

        [HttpGet("GetActiveInstagramPosts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveInstagramPosts()
        {
            var values = await _instagramPostService.GetActiveInstagramPostsAsync();
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstagramPostById(Guid id)
        {
            var value = await _instagramPostService.GetByIdInstagramPostAsync(id);
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInstagramPost(CreateInstagramPostDto createInstagramPostDto)
        {
            await _instagramPostService.CreateInstagramPostAsync(createInstagramPostDto);
            return Ok("Instagram gönderisi başarıyla eklendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstagramPost(Guid id)
        {
            await _instagramPostService.DeleteInstagramPostAsync(id);
            return Ok("Instagram gönderisi başarıyla silindi");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateInstagramPost(UpdateInstagramPostDto updateInstagramPostDto)
        {
            await _instagramPostService.UpdateInstagramPostAsync(updateInstagramPostDto);
            return Ok("Instagram gönderisi başarıyla güncellendi");
        }
    }
}

