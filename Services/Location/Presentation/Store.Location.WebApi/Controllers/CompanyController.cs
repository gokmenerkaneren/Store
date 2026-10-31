using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.CompanyCommands;
using Store.Location.Application.Features.CQRS.Handlers.CompanyHandlers;
using Store.Location.Application.Features.CQRS.Queries.CompanyQueries;

namespace Store.Location.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly GetCompaniesQueryHandler _getCompaniesQueryHandler;
        private readonly GetCompanyByIdQueryHandler _getCompanyByIdQueryHandler;
        private readonly CreateCompanyCommandHandler _createCompanyCommandHandler;
        private readonly UpdateCompanyCommandHandler _updateCompanyCommandHandler;
        private readonly RemoveCompanyCommandHandler _deleteCompanyCommandHandler;
        private readonly SoftDeleteCompanyCommandHandler _softDeleteCompanyCommandHandler;

        private readonly RestoreCompanyCommandHandler _restoreCompanyCommandHandler;

        public CompanyController(
            ILogger<CompanyController> logger,
            GetCompaniesQueryHandler getCompaniesQueryHandler,
            GetCompanyByIdQueryHandler getCompanyByIdQueryHandler,
            CreateCompanyCommandHandler createCompanyCommandHandler,
            UpdateCompanyCommandHandler updateCompanyCommandHandler,
            RemoveCompanyCommandHandler deleteCompanyCommandHandler,
            SoftDeleteCompanyCommandHandler softDeleteCompanyCommandHandler,
            RestoreCompanyCommandHandler restoreCompanyCommandHandler)
        {
            _logger = logger;
            _getCompaniesQueryHandler = getCompaniesQueryHandler;
            _getCompanyByIdQueryHandler = getCompanyByIdQueryHandler;
            _createCompanyCommandHandler = createCompanyCommandHandler;
            _updateCompanyCommandHandler = updateCompanyCommandHandler;
            _deleteCompanyCommandHandler = deleteCompanyCommandHandler;
            _softDeleteCompanyCommandHandler = softDeleteCompanyCommandHandler;
            _restoreCompanyCommandHandler = restoreCompanyCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> CompanyList([FromQuery] bool showDeleted = false)
        {
            var values = await _getCompaniesQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> CompanyListById(Guid id)
        {
            var values = await _getCompanyByIdQueryHandler.Handle(new GetCompanyByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(CreateCompanyCommand command)
        {
            try
            {
                await _createCompanyCommandHandler.Handle(command);
                return Ok("Company bilgisi başarılı bir şekilde eklendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompany(UpdateCompanyCommand command)
        {
            try
            {
                await _updateCompanyCommandHandler.Handle(command);
                return Ok("Company bilgisi başarılı bir şekilde güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCompany(Guid id)
        {
            await _deleteCompanyCommandHandler.Handle(new RemoveCompanyCommand(id));
            return Ok("Company bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteCompany(Guid id)
        {
            await _softDeleteCompanyCommandHandler.Handle(new SoftDeleteCompanyCommand(id));
            return Ok("Company bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreCompany(Guid id)
        {
            await _restoreCompanyCommandHandler.Handle(new RestoreCompanyCommand(id));
            return Ok("Company bilgisi başarılı bir şekilde geri alındı (Restore).");
        }
    }
}

