using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers;
using Store.Location.Application.Features.CQRS.Queries.UserAddressQueries;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Store.Location.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly ILogger<UserAddressController> _logger;
        private readonly GetUserAddressesQueryHandler _getUserAddressesQueryHandler;
        private readonly GetUserAddressByIdQueryHandler _getUserAddressByIdQueryHandler;
        private readonly CreateUserAddressCommandHandler _createUserAddressCommandHandler;
        private readonly UpdateUserAddressCommandHandler _updateUserAddressCommandHandler;
        private readonly RemoveUserAddressCommandHandler _deleteUserAddressCommandHandler;
        private readonly SoftDeleteUserAddressCommandHandler _softDeleteUserAddressCommandHandler;
        private readonly AnonymizeUserAddressCommandHandler _anonymizeUserAddressCommandHandler;
        private readonly SetDefaultUserAddressCommandHandler _setDefaultUserAddressCommandHandler;

        public UserAddressController(
            ILogger<UserAddressController> logger,
            GetUserAddressesQueryHandler getUserAddressesQueryHandler,
            GetUserAddressByIdQueryHandler getUserAddressByIdQueryHandler,
            CreateUserAddressCommandHandler createUserAddressCommandHandler,
            UpdateUserAddressCommandHandler updateUserAddressCommandHandler,
            RemoveUserAddressCommandHandler deleteUserAddressCommandHandler,
            SoftDeleteUserAddressCommandHandler softDeleteUserAddressCommandHandler,
            AnonymizeUserAddressCommandHandler anonymizeUserAddressCommandHandler,
            SetDefaultUserAddressCommandHandler setDefaultUserAddressCommandHandler)
        {
            _logger = logger;
            _getUserAddressesQueryHandler = getUserAddressesQueryHandler;
            _getUserAddressByIdQueryHandler = getUserAddressByIdQueryHandler;
            _createUserAddressCommandHandler = createUserAddressCommandHandler;
            _updateUserAddressCommandHandler = updateUserAddressCommandHandler;
            _deleteUserAddressCommandHandler = deleteUserAddressCommandHandler;
            _softDeleteUserAddressCommandHandler = softDeleteUserAddressCommandHandler;
            _anonymizeUserAddressCommandHandler = anonymizeUserAddressCommandHandler;
            _setDefaultUserAddressCommandHandler = setDefaultUserAddressCommandHandler;
        }

        private Guid? GetCurrentUserId()
        {
            var claimVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? User.FindFirst("sub")?.Value;
            if (Guid.TryParse(claimVal, out var guid))
            {
                return guid;
            }
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> UserAddressList([FromQuery] bool showDeleted = false)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            var values = await _getUserAddressesQueryHandler.Handle(showDeleted);

            // IDOR koruması: Admin hariç kullanıcı yalnızca kendi adreslerini görebilir
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                values = values.Where(x => x.CustomerId == currentUserId.Value).ToList();
            }

            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> UserAddressListById(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            var value = await _getUserAddressByIdQueryHandler.Handle(new GetUserAddressByIdQuery(id));
            if (value == null) return NotFound();

            // IDOR koruması: Kullanıcı yalnızca kendi adresini görüntüleyebilir
            if (value.CustomerId != currentUserId.Value && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAddress(CreateUserAddressCommand command)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            // IDOR koruması: CustomerId istemci verisinden değil token'dan atanır
            command.CustomerId = currentUserId.Value;

            try
            {
                await _createUserAddressCommandHandler.Handle(command);
                return Ok("UserAddress bilgisi başarılı bir şekilde eklendi");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Mükerrer adres başlığı: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserAddress(UpdateUserAddressCommand command)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            var existing = await _getUserAddressByIdQueryHandler.Handle(new GetUserAddressByIdQuery(command.Id));
            if (existing == null) return NotFound();

            // IDOR koruması: Başka kullanıcının adresi güncellenemez
            if (existing.CustomerId != currentUserId.Value && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            // CustomerId token'dan güvenceye alınır
            command.CustomerId = currentUserId.Value;

            try
            {
                await _updateUserAddressCommandHandler.Handle(command);
                return Ok("UserAddress bilgisi başarılı bir şekilde güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Mükerrer adres başlığı: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("set-default/{id}")]
        public async Task<IActionResult> SetDefaultUserAddress(Guid id, [FromQuery] Guid customerId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            var existing = await _getUserAddressByIdQueryHandler.Handle(new GetUserAddressByIdQuery(id));
            if (existing == null) return NotFound();

            // IDOR koruması: Başka kullanıcının adresi varsayılan yapılamaz
            if (existing.CustomerId != currentUserId.Value && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            try
            {
                // Query string'deki customerId manipülasyonuna karşı token'daki kimlik kullanılır
                await _setDefaultUserAddressCommandHandler.Handle(new SetDefaultUserAddressCommand(id, currentUserId.Value));
                return Ok("Varsayılan teslimat adresi başarıyla güncellendi.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Varsayılan adres ayarlanırken hata oluştu.");
                return StatusCode(500, "Varsayılan adres güncellenemedi.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUserAddress(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            var existing = await _getUserAddressByIdQueryHandler.Handle(new GetUserAddressByIdQuery(id));
            if (existing == null) return NotFound();

            if (existing.CustomerId != currentUserId.Value && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            await _deleteUserAddressCommandHandler.Handle(new RemoveUserAddressCommand(id));
            return Ok("UserAddress bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteUserAddress(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            var existing = await _getUserAddressByIdQueryHandler.Handle(new GetUserAddressByIdQuery(id));
            if (existing == null) return NotFound();

            if (existing.CustomerId != currentUserId.Value && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            await _softDeleteUserAddressCommandHandler.Handle(new SoftDeleteUserAddressCommand(id));
            return Ok("UserAddress bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        [HttpPost("anonymize/{id}")]
        public async Task<IActionResult> AnonymizeUserAddress(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Geçersiz kullanıcı oturumu.");

            var existing = await _getUserAddressByIdQueryHandler.Handle(new GetUserAddressByIdQuery(id));
            if (existing == null) return NotFound();

            if (existing.CustomerId != currentUserId.Value && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

            await _anonymizeUserAddressCommandHandler.Handle(new AnonymizeUserAddressCommand(id));
            return Ok("UserAddress bilgisi başarılı bir şekilde anonimleştirildi.");
        }
    }
}
