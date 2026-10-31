using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DtoLayer.CatalogDtos.InstagramPostDtos;
using Store.WebUI.Services.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Route("Admin/InstagramPost")]
    public class InstagramPostController : Controller
    {
        private readonly IInstagramPostService _instagramPostService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpClientFactory _httpClientFactory;

        public InstagramPostController(IInstagramPostService instagramPostService, IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClientFactory)
        {
            _instagramPostService = instagramPostService;
            _webHostEnvironment = webHostEnvironment;
            _httpClientFactory = httpClientFactory;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var values = await _instagramPostService.GetAllInstagramPostsAsync();
            return View(values);
        }

        [HttpGet]
        [Route("CreateInstagramPost")]
        public async Task<IActionResult> CreateInstagramPost()
        {
            int nextOrderIndex = 1;
            try
            {
                var values = await _instagramPostService.GetAllInstagramPostsAsync();
                if (values != null && values.Any())
                {
                    nextOrderIndex = values.Max(x => x.OrderIndex) + 1;
                }
            }
            catch
            {
                // API hata verirse varsayılan 1
            }

            var dto = new CreateInstagramPostDto { OrderIndex = nextOrderIndex, IsActive = true };
            return View(dto);
        }

        [HttpPost]
        [Route("CreateInstagramPost")]
        public async Task<IActionResult> CreateInstagramPost(CreateInstagramPostDto createInstagramPostDto)
        {
            createInstagramPostDto.IsActive = true;
            await _instagramPostService.CreateInstagramPostAsync(createInstagramPostDto);
            return RedirectToAction("Index", "InstagramPost", new { area = "Admin" });
        }

        [Route("DeleteInstagramPost/{id}")]
        public async Task<IActionResult> DeleteInstagramPost(Guid id)
        {
            await _instagramPostService.DeleteInstagramPostAsync(id);
            return RedirectToAction("Index", "InstagramPost", new { area = "Admin" });
        }

        [HttpGet]
        [Route("UpdateInstagramPost/{id}")]
        public async Task<IActionResult> UpdateInstagramPost(Guid id)
        {
            var value = await _instagramPostService.GetInstagramPostByIdAsync(id);
            return View(value);
        }

        [HttpPost]
        [Route("UpdateInstagramPost/{id}")]
        public async Task<IActionResult> UpdateInstagramPost(UpdateInstagramPostDto updateInstagramPostDto)
        {
            await _instagramPostService.UpdateInstagramPostAsync(updateInstagramPostDto);
            return RedirectToAction("Index", "InstagramPost", new { area = "Admin" });
        }

        [HttpPost]
        [Route("FetchInstagramMedia")]
        public async Task<IActionResult> FetchInstagramMedia([FromBody] FetchInstagramMediaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.PostUrl))
                    return Json(new { success = false, message = "URL boş olamaz." });

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var response = await client.GetAsync(request.PostUrl);
                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, message = "Instagram'a erişilemedi." });

                var html = await response.Content.ReadAsStringAsync();
                
                // og:image etiketini bul
                var match = Regex.Match(html, "<meta property=\"og:image\" content=\"(.*?)\"");
                if (!match.Success)
                    return Json(new { success = false, message = "Medya bulunamadı veya Instagram engelledi." });

                string imageUrl = match.Groups[1].Value.Replace("&amp;", "&");

                // Resmi indir
                var imageResponse = await client.GetAsync(imageUrl);
                if (!imageResponse.IsSuccessStatusCode)
                    return Json(new { success = false, message = "Resim indirilemedi." });

                var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
                
                // Kaydet
                var fileName = Guid.NewGuid().ToString() + ".jpg";
                var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Instagram");
                
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var filePath = Path.Combine(directoryPath, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                return Json(new { success = true, url = "/Images/Instagram/" + fileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class FetchInstagramMediaRequest
    {
        public string PostUrl { get; set; }
    }
}

