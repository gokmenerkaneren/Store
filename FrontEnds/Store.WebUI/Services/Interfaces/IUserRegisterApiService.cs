using Refit;
using Store.WebUI.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IUserRegisterApiService
    {
        [Post("/api/UserRegister")]
        Task<HttpResponseMessage> RegisterUserAsync([Body] UserRegisterViewModel model);
    }
}
