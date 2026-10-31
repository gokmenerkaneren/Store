using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.CityCommands;
using Store.Location.Application.Features.CQRS.Handlers.CityHandlers;
using Store.Location.Application.Features.CQRS.Queries.CityQueries;

namespace Store.Location.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ILogger<CityController> _logger;
        private readonly GetCitiesQueryHandler _getCitiesQueryHandler;
        private readonly GetCityByIdQueryHandler _getCityByIdQueryHandler;
        private readonly CreateCityCommandHandler _createCityCommandHandler;
        private readonly UpdateCityCommandHandler _updateCityCommandHandler;
        private readonly RemoveCityCommandHandler _deleteCityCommandHandler;
        private readonly SoftDeleteCityCommandHandler _softDeleteCityCommandHandler;
        private readonly RestoreCityCommandHandler _restoreCityCommandHandler;

        public CityController(
            ILogger<CityController> logger,
            GetCitiesQueryHandler getCitiesQueryHandler,
            GetCityByIdQueryHandler getCityByIdQueryHandler,
            CreateCityCommandHandler createCityCommandHandler,
            UpdateCityCommandHandler updateCityCommandHandler,
            RemoveCityCommandHandler deleteCityCommandHandler,
            SoftDeleteCityCommandHandler softDeleteCityCommandHandler,
            RestoreCityCommandHandler restoreCityCommandHandler)
        {
            _logger = logger;
            _getCitiesQueryHandler = getCitiesQueryHandler;
            _getCityByIdQueryHandler = getCityByIdQueryHandler;
            _createCityCommandHandler = createCityCommandHandler;
            _updateCityCommandHandler = updateCityCommandHandler;
            _deleteCityCommandHandler = deleteCityCommandHandler;
            _softDeleteCityCommandHandler = softDeleteCityCommandHandler;
            _restoreCityCommandHandler = restoreCityCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> CityList([FromQuery] bool showDeleted = false)
        {
            var values = await _getCitiesQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> CityListById(Guid id)
        {
            var values = await _getCityByIdQueryHandler.Handle(new GetCityByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity(CreateCityCommand command)
        {
            try
            {
                await _createCityCommandHandler.Handle(command);
                return Ok("City bilgisi başarılı bir şekilde eklendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCity(UpdateCityCommand command)
        {
            try
            {
                await _updateCityCommandHandler.Handle(command);
                return Ok("City bilgisi başarılı bir şekilde güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCity(Guid id)
        {
            await _deleteCityCommandHandler.Handle(new RemoveCityCommand(id));
            return Ok("City bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteCity(Guid id)
        {
            await _softDeleteCityCommandHandler.Handle(new SoftDeleteCityCommand(id));
            return Ok("City bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreCity(Guid id)
        {
            await _restoreCityCommandHandler.Handle(new RestoreCityCommand(id));
            return Ok("City bilgisi başarılı bir şekilde geri alındı (Restore).");
        }
    }
}
