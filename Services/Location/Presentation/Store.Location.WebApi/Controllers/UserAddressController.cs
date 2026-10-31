using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers;
using Store.Location.Application.Features.CQRS.Queries.UserAddressQueries;
using System;
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

        [HttpGet]
        public async Task<IActionResult> UserAddressList([FromQuery] bool showDeleted = false)
        {
            var values = await _getUserAddressesQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> UserAddressListById(Guid id)
        {
            var values = await _getUserAddressByIdQueryHandler.Handle(new GetUserAddressByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAddress(CreateUserAddressCommand command)
        {
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
            try
            {
                await _setDefaultUserAddressCommandHandler.Handle(new SetDefaultUserAddressCommand(id, customerId));
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
            await _deleteUserAddressCommandHandler.Handle(new RemoveUserAddressCommand(id));
            return Ok("UserAddress bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteUserAddress(Guid id)
        {
            await _softDeleteUserAddressCommandHandler.Handle(new SoftDeleteUserAddressCommand(id));
            return Ok("UserAddress bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        [HttpPost("anonymize/{id}")]
        public async Task<IActionResult> AnonymizeUserAddress(Guid id)
        {
            await _anonymizeUserAddressCommandHandler.Handle(new AnonymizeUserAddressCommand(id));
            return Ok("UserAddress bilgisi başarılı bir şekilde anonimleştirildi.");
        }
    }
}
