using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Order.Application.Features.Mediator.Commands.OrderingCommands;
using Store.Order.Application.Features.Mediator.Queries.OrderingQueries;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Store.Order.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                              ?? User.FindFirst("sub")?.Value;

            if (Guid.TryParse(userIdClaim, out var parsedGuid))
            {
                return parsedGuid;
            }
            return null;
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        [HttpGet]
        public async Task<IActionResult> OrderingList()
        {
            var values = await _mediator.Send(new GetOrderingQuery());
            
            // Güvenlik K7: Admin olmayan kullanıcılar sadece kendilerine ait siparişleri görebilir.
            if (!IsAdmin())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue)
                {
                    return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
                }
                values = values.Where(x => x.UserId == currentUserId.Value).ToList();
            }

            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderingById(Guid id)
        {
            var value = await _mediator.Send(new GetOrderingByIdQuery(id));
            if (value == null)
            {
                return NotFound("Sipariş bulunamadı.");
            }

            // Güvenlik K7 (IDOR Koruması): Başka kullanıcının siparişine erişim engellenir
            if (!IsAdmin())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue || value.UserId != currentUserId.Value)
                {
                    return Forbid();
                }
            }

            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrdering(CreateOrderingCommand command)
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            }

            // Güvenlik K7: UserId istemci gövdesinden kabul edilmez; JWT sub üzerinden bağlanır
            command.UserId = currentUserId.Value;

            if (command.TotalPrice < 0)
            {
                return BadRequest("Sipariş toplam tutarı negatif olamaz.");
            }

            await _mediator.Send(command);
            return Ok("Sipariş başarılı bir şekilde oluşturuldu.");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveOrdering(Guid id)
        {
            var existingOrder = await _mediator.Send(new GetOrderingByIdQuery(id));
            if (existingOrder == null)
            {
                return NotFound("Silinecek sipariş bulunamadı.");
            }

            // Güvenlik K7 (IDOR Koruması): Kullanıcı sadece kendi siparişini silebilir
            if (!IsAdmin())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue || existingOrder.UserId != currentUserId.Value)
                {
                    return Forbid();
                }
            }

            await _mediator.Send(new RemoveOrderingCommand(id));
            return Ok("Sipariş başarılı bir şekilde silindi.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrdering(UpdateOrderingCommand command)
        {
            var existingOrder = await _mediator.Send(new GetOrderingByIdQuery(command.Id));
            if (existingOrder == null)
            {
                return NotFound("Güncellenecek sipariş bulunamadı.");
            }

            // Güvenlik K7 (IDOR Koruması): Kullanıcı sadece kendi siparişini güncelleyebilir
            if (!IsAdmin())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue || existingOrder.UserId != currentUserId.Value)
                {
                    return Forbid();
                }
            }

            await _mediator.Send(command);
            return Ok("Sipariş başarılı bir şekilde güncellendi.");
        }
    }
}
