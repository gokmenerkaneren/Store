using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands;
using Store.Location.Application.Features.CQRS.Handlers.NeighborhoodHandlers;
using Store.Location.Application.Features.CQRS.Queries.NeighborhoodQueries;

namespace Store.Location.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class NeighborhoodController : ControllerBase
    {
        private readonly ILogger<NeighborhoodController> _logger;
        private readonly GetNeighborhoodsQueryHandler _getNeighborhoodsQueryHandler;
        private readonly GetNeighborhoodByIdQueryHandler _getNeighborhoodByIdQueryHandler;
        private readonly GetNeighborhoodsByDistrictIdQueryHandler _getNeighborhoodsByDistrictIdQueryHandler;
        private readonly CreateNeighborhoodCommandHandler _createNeighborhoodCommandHandler;
        private readonly UpdateNeighborhoodCommandHandler _updateNeighborhoodCommandHandler;
        private readonly RemoveNeighborhoodCommandHandler _deleteNeighborhoodCommandHandler;
        private readonly SoftDeleteNeighborhoodCommandHandler _softDeleteNeighborhoodCommandHandler;
        private readonly RestoreNeighborhoodCommandHandler _restoreNeighborhoodCommandHandler;

        public NeighborhoodController(
            ILogger<NeighborhoodController> logger,
            GetNeighborhoodsQueryHandler getNeighborhoodsQueryHandler,
            GetNeighborhoodByIdQueryHandler getNeighborhoodByIdQueryHandler,
            GetNeighborhoodsByDistrictIdQueryHandler getNeighborhoodsByDistrictIdQueryHandler,
            CreateNeighborhoodCommandHandler createNeighborhoodCommandHandler,
            UpdateNeighborhoodCommandHandler updateNeighborhoodCommandHandler,
            RemoveNeighborhoodCommandHandler deleteNeighborhoodCommandHandler,
            SoftDeleteNeighborhoodCommandHandler softDeleteNeighborhoodCommandHandler,
            RestoreNeighborhoodCommandHandler restoreNeighborhoodCommandHandler)
        {
            _logger = logger;
            _getNeighborhoodsQueryHandler = getNeighborhoodsQueryHandler;
            _getNeighborhoodByIdQueryHandler = getNeighborhoodByIdQueryHandler;
            _getNeighborhoodsByDistrictIdQueryHandler = getNeighborhoodsByDistrictIdQueryHandler;
            _createNeighborhoodCommandHandler = createNeighborhoodCommandHandler;
            _updateNeighborhoodCommandHandler = updateNeighborhoodCommandHandler;
            _deleteNeighborhoodCommandHandler = deleteNeighborhoodCommandHandler;
            _softDeleteNeighborhoodCommandHandler = softDeleteNeighborhoodCommandHandler;
            _restoreNeighborhoodCommandHandler = restoreNeighborhoodCommandHandler;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> NeighborhoodList([FromQuery] bool showDeleted = false)
        {
            var values = await _getNeighborhoodsQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> NeighborhoodListById(Guid id)
        {
            var values = await _getNeighborhoodByIdQueryHandler.Handle(new GetNeighborhoodByIdQuery(id));
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("ByDistrictId/{districtId}")]
        public async Task<IActionResult> NeighborhoodsByDistrictId(Guid districtId, [FromQuery] bool showDeleted = false)
        {
            var values = await _getNeighborhoodsByDistrictIdQueryHandler.Handle(new GetNeighborhoodsByDistrictIdQuery(districtId, showDeleted));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNeighborhood(CreateNeighborhoodCommand command)
        {
            try
            {
                await _createNeighborhoodCommandHandler.Handle(command);
                return Ok("Neighborhood bilgisi başarılı bir şekilde eklendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNeighborhood(UpdateNeighborhoodCommand command)
        {
            try
            {
                await _updateNeighborhoodCommandHandler.Handle(command);
                return Ok("Neighborhood bilgisi başarılı bir şekilde güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveNeighborhood(Guid id)
        {
            await _deleteNeighborhoodCommandHandler.Handle(new RemoveNeighborhoodCommand(id));
            return Ok("Neighborhood bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteNeighborhood(Guid id)
        {
            await _softDeleteNeighborhoodCommandHandler.Handle(new SoftDeleteNeighborhoodCommand(id));
            return Ok("Neighborhood bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreNeighborhood(Guid id)
        {
            await _restoreNeighborhoodCommandHandler.Handle(new RestoreNeighborhoodCommand(id));
            return Ok("Neighborhood bilgisi başarılı bir şekilde geri alındı (Restore).");
        }
    }
}

