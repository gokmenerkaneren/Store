using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands;
using Store.Location.Application.Features.CQRS.Handlers.BranchAddressHandlers;
using Store.Location.Application.Features.CQRS.Queries.BranchAddress;

namespace Store.Location.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchAddressController : ControllerBase
    {
        private readonly ILogger<BranchAddressController> _logger;
        private readonly GetBranchAddressesQueryHandler _getBranchAddressesQueryHandler;
        private readonly GetBranchAddressByIdQueryHandler _getBranchAddressByIdQueryHandler;
        private readonly CreateBranchAddressCommandHandler _createBranchAddressCommandHandler;
        private readonly UpdateBranchAddressCommandHandler _updateBranchAddressCommandHandler;
        private readonly RemoveBranchAddressCommandHandler _deleteBranchAddressCommandHandler;
        private readonly SoftDeleteBranchAddressCommandHandler _softDeleteBranchAddressCommandHandler;

        public BranchAddressController(
            ILogger<BranchAddressController> logger,
            GetBranchAddressesQueryHandler getBranchAddressesQueryHandler,
            GetBranchAddressByIdQueryHandler getBranchAddressByIdQueryHandler,
            CreateBranchAddressCommandHandler createBranchAddressCommandHandler,
            UpdateBranchAddressCommandHandler updateBranchAddressCommandHandler,
            RemoveBranchAddressCommandHandler deleteBranchAddressCommandHandler,
            SoftDeleteBranchAddressCommandHandler softDeleteBranchAddressCommandHandler)
        {
            _logger = logger;
            _getBranchAddressesQueryHandler = getBranchAddressesQueryHandler;
            _getBranchAddressByIdQueryHandler = getBranchAddressByIdQueryHandler;
            _createBranchAddressCommandHandler = createBranchAddressCommandHandler;
            _updateBranchAddressCommandHandler = updateBranchAddressCommandHandler;
            _deleteBranchAddressCommandHandler = deleteBranchAddressCommandHandler;
            _softDeleteBranchAddressCommandHandler = softDeleteBranchAddressCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> BranchAddressList([FromQuery] bool showDeleted = false)
        {
            var values = await _getBranchAddressesQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BranchAddressListById(Guid id)
        {
            var values = await _getBranchAddressByIdQueryHandler.Handle(new GetBranchAddressByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranchAddress(CreateBranchAddressCommand command)
        {
            await _createBranchAddressCommandHandler.Handle(command);
            return Ok("BranchAddress bilgisi başarılı bir şekilde eklendi");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBranchAddress(UpdateBranchAddressCommand command)
        {
            await _updateBranchAddressCommandHandler.Handle(command);
            return Ok("BranchAddress bilgisi başarılı bir şekilde güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveBranchAddress(Guid id)
        {
            await _deleteBranchAddressCommandHandler.Handle(new RemoveBranchAddressCommand(id));
            return Ok("BranchAddress bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteBranchAddress(Guid id)
        {
            await _softDeleteBranchAddressCommandHandler.Handle(new SoftDeleteBranchAddressCommand(id));
            return Ok("BranchAddress bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }
    }
}

