using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Rental.Application.Features.RentalOrders.Commands.CreateRentalOrder;
using System.Threading.Tasks;

namespace Store.Rental.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RentalsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRentalOrder([FromBody] CreateRentalOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { RentalOrderId = result });
        }
    }
}
