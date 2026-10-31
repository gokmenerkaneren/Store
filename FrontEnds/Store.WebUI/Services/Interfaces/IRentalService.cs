using System.Threading.Tasks;
using Refit;
using Store.WebUI.Models.Rental;

namespace Store.WebUI.Services.Interfaces
{
    public interface IRentalService
    {
        [Post("/api/Rentals")]
        Task<string> CreateRentalOrderAsync([Body] CreateRentalViewModel model);
    }
}
