using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Order.Application.Features.CQRS.Commands.AddressCommands;
using Store.Order.Application.Features.CQRS.Handlers.AddressHandlers;
using Store.Order.Application.Features.CQRS.Queries.AddressQueries;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Store.Order.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly ILogger<AddressesController> _logger;
        private readonly GetAddressQueryHandler _getAddressQueryHandler;
        private readonly GetAddressByIdQueryHandler _getAddressByIdQueryHandler;
        private readonly CreateAddressCommandHandler _createAddressCommandHandler;
        private readonly UpdateAddressCommandHandler _updateAddressCommandHandler;
        private readonly RemoveAddressCommandHandler _removeAddressCommandHandler;

        public AddressesController(
            ILogger<AddressesController> logger, 
            GetAddressQueryHandler getAddressQueryHandler, 
            GetAddressByIdQueryHandler getAddressByIdQueryHandler, 
            CreateAddressCommandHandler createAddressCommandHandler, 
            UpdateAddressCommandHandler updateAddressCommandHandler, 
            RemoveAddressCommandHandler removeAddressCommandHandler)
        {
            _logger = logger;
            _getAddressQueryHandler = getAddressQueryHandler;
            _getAddressByIdQueryHandler = getAddressByIdQueryHandler;
            _createAddressCommandHandler = createAddressCommandHandler;
            _updateAddressCommandHandler = updateAddressCommandHandler;
            _removeAddressCommandHandler = removeAddressCommandHandler;
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
        public async Task<IActionResult> AdressList()
        {
            var values = await _getAddressQueryHandler.Handle();
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
        public async Task<IActionResult> AdressListById(Guid id)
        {
            var values = await _getAddressByIdQueryHandler.Handle(new GetAddressByIdQuery(id));
            if (values == null)
            {
                return NotFound("Adres bulunamadı.");
            }

            if (!IsAdmin())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue || values.UserId != currentUserId.Value)
                {
                    return Forbid();
                }
            }

            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(CreateAddressCommand command)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId.HasValue)
            {
                command.UserId = currentUserId.Value;
            }

            await _createAddressCommandHandler.Handle(command);
            return Ok("Adres bilgisi başarili bir şekilde eklendi");
        }

        [HttpPut]   
        public async Task<IActionResult> UpdateAddress(UpdateAddressCommand command)
        {
            var existingAddress = await _getAddressByIdQueryHandler.Handle(new GetAddressByIdQuery(command.Id));
            if (existingAddress == null)
            {
                return NotFound("Güncellenecek adres bulunamadı.");
            }

            if (!IsAdmin())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue || existingAddress.UserId != currentUserId.Value)
                {
                    return Forbid();
                }
            }

            await _updateAddressCommandHandler.Handle(command);
            return Ok("Adres bilgisi başarili bir şekilde güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAddress(Guid id)
        {
            var existingAddress = await _getAddressByIdQueryHandler.Handle(new GetAddressByIdQuery(id));
            if (existingAddress == null)
            {
                return NotFound("Silinecek adres bulunamadı.");
            }

            if (!IsAdmin())
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue || existingAddress.UserId != currentUserId.Value)
                {
                    return Forbid();
                }
            }

            await _removeAddressCommandHandler.Handle(new RemoveAddressCommand(id));
            return Ok("Adres bilgisi başarili bir şekilde silindi");
        }
    }
}
