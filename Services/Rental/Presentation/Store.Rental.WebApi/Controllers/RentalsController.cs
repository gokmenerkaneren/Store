using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Rental.Application.Features.RentalOrders.Commands.CreateRentalOrder;
using System;
using System.Security.Claims;
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            }

            // Güvenlik K7: UserId istemci gövdesinden kabul edilmez; JWT token (sub) üzerinden bağlanır.
            command.UserId = userId;

            var result = await _mediator.Send(command);
            return Ok(new { RentalOrderId = result });
        }
    }
}
