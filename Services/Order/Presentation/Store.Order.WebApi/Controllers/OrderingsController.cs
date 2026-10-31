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
        public async Task<IActionResult> CreateOrdering([FromBody] CreateOrderingCommand command)
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized("Kullanıcı kimliği doğrulanamadı.");
            }

            // Güvenlik K7: UserId istemci gövdesinden kabul edilmez; JWT sub üzerinden bağlanır
            command.UserId = currentUserId.Value;

            // Güvenlik İ3: Müşteri siparişi doğrudan Delivered vb. statüde açamaz, varsayılan Pending'dir
            if (!IsAdmin())
            {
                command.Status = Store.Order.Domain.Enums.OrderStatus.Pending;
            }

            if (command.TotalPrice < 0)
            {
                return BadRequest("Sipariş toplam tutarı negatif olamaz.");
            }

            // İ2, İ3 & İ6: Sipariş kalemleri ve stok doğrulaması ile birlikte atomik oluşturulup ID'si döndürülür
            try
            {
                var orderId = await _mediator.Send(command);
                return Ok(new { OrderId = orderId, Message = "Sipariş başarılı bir şekilde oluşturuldu." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
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

                // İ3: İşleme alınmış, kargolanmış veya teslim edilmiş siparişler silinemez
                if (existingOrder.Status != Store.Order.Domain.Enums.OrderStatus.Pending)
                {
                    return BadRequest("İşleme alınmış veya kargoya verilmiş siparişler silinemez. İade/iptal talebi oluşturunuz.");
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

                // Güvenlik K7: Admin olmayan kullanıcı Status, TotalPrice ve UserId alanlarını değiştiremez;
                // istek gövdesindeki değerler yok sayılır ve mevcut sipariş değerleri korunur.
                command.UserId = existingOrder.UserId;
                command.TotalPrice = existingOrder.TotalPrice;
                command.Status = existingOrder.Status;
            }

            try
            {
                await _mediator.Send(command);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

            return Ok("Sipariş başarılı bir şekilde güncellendi.");
        }
    }
}
