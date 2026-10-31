using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.CountryCommands;
using Store.Location.Application.Features.CQRS.Handlers.CountryHandlers;
using Store.Location.Application.Features.CQRS.Queries.CountryQueries;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Store.Location.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ILogger<CountryController> _logger;
        private readonly GetCountriesQueryHandler _getCountriesQueryHandler;
        private readonly GetCountryByIdQueryHandler _getCountryByIdQueryHandler;
        private readonly CreateCountryCommandHandler _createCountryCommandHandler;
        private readonly UpdateCountryCommandHandler _updateCountryCommandHandler;
        private readonly RemoveCountryCommandHandler _deleteCountryCommandHandler;
        private readonly SoftDeleteCountryCommandHandler _softDeleteCountryCommandHandler;
        private readonly RestoreCountryCommandHandler _restoreCountryCommandHandler;


        public CountryController(ILogger<CountryController> logger, GetCountriesQueryHandler getCountriesQueryHandler, GetCountryByIdQueryHandler getCountryByIdQueryHandler, CreateCountryCommandHandler createCountryCommandHandler, UpdateCountryCommandHandler updateCountryCommandHandler, RemoveCountryCommandHandler deleteCountryCommandHandler, SoftDeleteCountryCommandHandler softDeleteCountryCommandHandler, RestoreCountryCommandHandler restoreCountryCommandHandler)
        {
            _logger = logger;
            _getCountriesQueryHandler = getCountriesQueryHandler;
            _getCountryByIdQueryHandler = getCountryByIdQueryHandler;
            _createCountryCommandHandler = createCountryCommandHandler;
            _updateCountryCommandHandler = updateCountryCommandHandler;
            _deleteCountryCommandHandler = deleteCountryCommandHandler;
            _softDeleteCountryCommandHandler = softDeleteCountryCommandHandler;
            _restoreCountryCommandHandler = restoreCountryCommandHandler;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CountryList([FromQuery] bool showDeleted = false)
        {
            var values = await _getCountriesQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> CountryListById(Guid id)
        {
            var values = await _getCountryByIdQueryHandler.Handle(new GetCountryByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountry(CreateCountryCommand command)
        {
            try
            {
                await _createCountryCommandHandler.Handle(command);
                return Ok("Country bilgisi başarılı bir şekilde eklendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCountry(UpdateCountryCommand command)
        {
            try
            {
                await _updateCountryCommandHandler.Handle(command);
                return Ok("Country bilgisi başarılı bir şekilde güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCountry(Guid id)
        {
            await _deleteCountryCommandHandler.Handle(new RemoveCountryCommand(id));
            return Ok("Country bilgisi başarılı bir şekilde silindi");
        }

        // Soft Delete için Route'u değiştiriyoruz (Örn: /api/Country/soft-delete/5)
        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteCountry(Guid id)
        {
            // Command nesnesinin ismini kendi projene göre uyarlayabilirsin
            await _softDeleteCountryCommandHandler.Handle(new SoftDeleteCountryCommand(id));
            return Ok("Country bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        // Restore (Geri Alma) için endpoint
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreCountry(Guid id)
        {
            await _restoreCountryCommandHandler.Handle(new RestoreCountryCommand(id));
            return Ok("Country bilgisi başarılı bir şekilde geri alındı (Restore).");
        }

    }
}
