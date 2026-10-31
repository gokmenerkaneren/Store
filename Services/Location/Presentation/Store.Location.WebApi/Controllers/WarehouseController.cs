using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.WarehouseCommands;
using Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers;
using Store.Location.Application.Features.CQRS.Queries.WarehouseQueries;

namespace Store.Location.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly ILogger<WarehouseController> _logger;
        private readonly GetWarehousesQueryHandler _getWarehousesQueryHandler;
        private readonly GetWarehouseByIdQueryHandler _getWarehouseByIdQueryHandler;
        private readonly CreateWarehouseCommandHandler _createWarehouseCommandHandler;
        private readonly UpdateWarehouseCommandHandler _updateWarehouseCommandHandler;
        private readonly RemoveWarehouseCommandHandler _deleteWarehouseCommandHandler;
        private readonly SoftDeleteWarehouseCommandHandler _softDeleteWarehouseCommandHandler;
        private readonly RestoreWarehouseCommandHandler _restoreWarehouseCommandHandler;

        public WarehouseController(
            ILogger<WarehouseController> logger,
            GetWarehousesQueryHandler getWarehousesQueryHandler,
            GetWarehouseByIdQueryHandler getWarehouseByIdQueryHandler,
            CreateWarehouseCommandHandler createWarehouseCommandHandler,
            UpdateWarehouseCommandHandler updateWarehouseCommandHandler,
            RemoveWarehouseCommandHandler deleteWarehouseCommandHandler,
            SoftDeleteWarehouseCommandHandler softDeleteWarehouseCommandHandler,
            RestoreWarehouseCommandHandler restoreWarehouseCommandHandler)
        {
            _logger = logger;
            _getWarehousesQueryHandler = getWarehousesQueryHandler;
            _getWarehouseByIdQueryHandler = getWarehouseByIdQueryHandler;
            _createWarehouseCommandHandler = createWarehouseCommandHandler;
            _updateWarehouseCommandHandler = updateWarehouseCommandHandler;
            _deleteWarehouseCommandHandler = deleteWarehouseCommandHandler;
            _softDeleteWarehouseCommandHandler = softDeleteWarehouseCommandHandler;
            _restoreWarehouseCommandHandler = restoreWarehouseCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> WarehouseList([FromQuery] bool showDeleted = false)
        {
            var values = await _getWarehousesQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> WarehouseListById(Guid id)
        {
            var values = await _getWarehouseByIdQueryHandler.Handle(new GetWarehouseByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse(CreateWarehouseCommand command)
        {
            try
            {
                await _createWarehouseCommandHandler.Handle(command);
                return Ok("Warehouse bilgisi başarılı bir şekilde eklendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWarehouse(UpdateWarehouseCommand command)
        {
            try
            {
                await _updateWarehouseCommandHandler.Handle(command);
                return Ok("Warehouse bilgisi başarılı bir şekilde güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveWarehouse(Guid id)
        {
            try
            {
                await _deleteWarehouseCommandHandler.Handle(new RemoveWarehouseCommand(id));
                return Ok("Warehouse bilgisi başarılı bir şekilde silindi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteWarehouse(Guid id)
        {
            try
            {
                await _softDeleteWarehouseCommandHandler.Handle(new SoftDeleteWarehouseCommand(id));
                return Ok("Warehouse bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreWarehouse(Guid id)
        {
            await _restoreWarehouseCommandHandler.Handle(new RestoreWarehouseCommand(id));
            return Ok("Warehouse bilgisi başarılı bir şekilde geri alındı (Restore).");
        }
    }
}

