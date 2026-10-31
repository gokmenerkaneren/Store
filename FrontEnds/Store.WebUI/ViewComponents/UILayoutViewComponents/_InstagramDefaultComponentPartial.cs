using Microsoft.AspNetCore.Mvc;
using Store.WebUI.Services.Interfaces;
using System.Threading.Tasks;

namespace Store.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _InstagramDefaultComponentPartial : ViewComponent
    {
        private readonly IInstagramPostService _instagramPostService;

        public _InstagramDefaultComponentPartial(IInstagramPostService instagramPostService)
        {
            _instagramPostService = instagramPostService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _instagramPostService.GetActiveInstagramPostsAsync();
            return View(values);
        }
    }
}

