using Microsoft.AspNetCore.Mvc;
using MediatR;
using Store.Favorite.Domain.Entities;
using Store.Favorite.Application.Features.Mediator.Commands.FavoriteItemCommands;
using Store.Favorite.Application.Features.Mediator.Queries.FavoriteItemQueries;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;

namespace Store.Favorite.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FavoritesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid? GetCurrentUserId()
        {
            var claimVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (Guid.TryParse(claimVal, out var guid))
            {
                return guid;
            }
            return null;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserFavorites(Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            // IDOR koruması: Kullanıcı yalnızca kendi favorilerini görebilir (Admin hariç)
            if (currentUserId.Value != userId && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            var favorites = await _mediator.Send(new GetUserFavoritesQuery(userId));
            return Ok(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteCommand command)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            // IDOR koruması: Başkası adına favori eklenemez
            if (command.UserId != Guid.Empty && command.UserId != currentUserId.Value && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            command.UserId = currentUserId.Value;
            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFavorite([FromQuery] Guid userId, [FromQuery] string productId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            // IDOR koruması: Başkasının favorisi silinemez
            if (userId != currentUserId.Value && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            var command = new RemoveFavoriteCommand
            {
                UserId = currentUserId.Value,
                ProductId = productId
            };

            await _mediator.Send(command);
            return Ok();
        }
    }
}


