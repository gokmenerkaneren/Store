using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.DistrictCommands;
using Store.Location.Application.Features.CQRS.Handlers.DistrictHandlers;
using Store.Location.Application.Features.CQRS.Queries.DistrictQueries;

namespace Store.Location.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly ILogger<DistrictController> _logger;
        private readonly GetDistrictsQueryHandler _getDistrictsQueryHandler;
        private readonly GetDistrictByIdQueryHandler _getDistrictByIdQueryHandler;
        private readonly CreateDistrictCommandHandler _createDistrictCommandHandler;
        private readonly UpdateDistrictCommandHandler _updateDistrictCommandHandler;
        private readonly RemoveDistrictCommandHandler _deleteDistrictCommandHandler;
        private readonly SoftDeleteDistrictCommandHandler _softDeleteDistrictCommandHandler;
        private readonly RestoreDistrictCommandHandler _restoreDistrictCommandHandler;

        public DistrictController(
            ILogger<DistrictController> logger,
            GetDistrictsQueryHandler getDistrictsQueryHandler,
            GetDistrictByIdQueryHandler getDistrictByIdQueryHandler,
            CreateDistrictCommandHandler createDistrictCommandHandler,
            UpdateDistrictCommandHandler updateDistrictCommandHandler,
            RemoveDistrictCommandHandler deleteDistrictCommandHandler,
            SoftDeleteDistrictCommandHandler softDeleteDistrictCommandHandler,
            RestoreDistrictCommandHandler restoreDistrictCommandHandler)
        {
            _logger = logger;
            _getDistrictsQueryHandler = getDistrictsQueryHandler;
            _getDistrictByIdQueryHandler = getDistrictByIdQueryHandler;
            _createDistrictCommandHandler = createDistrictCommandHandler;
            _updateDistrictCommandHandler = updateDistrictCommandHandler;
            _deleteDistrictCommandHandler = deleteDistrictCommandHandler;
            _softDeleteDistrictCommandHandler = softDeleteDistrictCommandHandler;
            _restoreDistrictCommandHandler = restoreDistrictCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> DistrictList([FromQuery] bool showDeleted = false)
        {
            var values = await _getDistrictsQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DistrictListById(Guid id)
        {
            var values = await _getDistrictByIdQueryHandler.Handle(new GetDistrictByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDistrict(CreateDistrictCommand command)
        {
            try
            {
                await _createDistrictCommandHandler.Handle(command);
                return Ok("District bilgisi başarılı bir şekilde eklendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDistrict(UpdateDistrictCommand command)
        {
            try
            {
                await _updateDistrictCommandHandler.Handle(command);
                return Ok("District bilgisi başarılı bir şekilde güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveDistrict(Guid id)
        {
            await _deleteDistrictCommandHandler.Handle(new RemoveDistrictCommand(id));
            return Ok("District bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteDistrict(Guid id)
        {
            await _softDeleteDistrictCommandHandler.Handle(new SoftDeleteDistrictCommand(id));
            return Ok("District bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreDistrict(Guid id)
        {
            await _restoreDistrictCommandHandler.Handle(new RestoreDistrictCommand(id));
            return Ok("District bilgisi başarılı bir şekilde geri alındı (Restore).");
        }
    }
}

