using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.DealerCommands;
using Store.Location.Application.Features.CQRS.Handlers.DealerHandlers;
using Store.Location.Application.Features.CQRS.Queries.DealerQueries;

namespace Store.Location.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DealerController : ControllerBase
    {
        private readonly ILogger<DealerController> _logger;
        private readonly GetDealersQueryHandler _getDealersQueryHandler;
        private readonly GetDealerByIdQueryHandler _getDealerByIdQueryHandler;
        private readonly CreateDealerCommandHandler _createDealerCommandHandler;
        private readonly UpdateDealerCommandHandler _updateDealerCommandHandler;
        private readonly RemoveDealerCommandHandler _deleteDealerCommandHandler;
        private readonly SoftDeleteDealerCommandHandler _softDeleteDealerCommandHandler;
        private readonly RestoreDealerCommandHandler _restoreDealerCommandHandler;

        public DealerController(
            ILogger<DealerController> logger,
            GetDealersQueryHandler getDealersQueryHandler,
            GetDealerByIdQueryHandler getDealerByIdQueryHandler,
            CreateDealerCommandHandler createDealerCommandHandler,
            UpdateDealerCommandHandler updateDealerCommandHandler,
            RemoveDealerCommandHandler deleteDealerCommandHandler,
            SoftDeleteDealerCommandHandler softDeleteDealerCommandHandler,
            RestoreDealerCommandHandler restoreDealerCommandHandler)
        {
            _logger = logger;
            _getDealersQueryHandler = getDealersQueryHandler;
            _getDealerByIdQueryHandler = getDealerByIdQueryHandler;
            _createDealerCommandHandler = createDealerCommandHandler;
            _updateDealerCommandHandler = updateDealerCommandHandler;
            _deleteDealerCommandHandler = deleteDealerCommandHandler;
            _softDeleteDealerCommandHandler = softDeleteDealerCommandHandler;
            _restoreDealerCommandHandler = restoreDealerCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> DealerList([FromQuery] bool showDeleted = false)
        {
            var values = await _getDealersQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpPost("datatables")]
        public async Task<IActionResult> DealerListDataTables([FromServices] GetDealersDataTablesQueryHandler dataTablesHandler, [FromBody] Store.DtoLayer.CommonDtos.DataTablesRequest request, [FromQuery] bool showDeleted = false)
        {
            var values = await dataTablesHandler.Handle(request, showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DealerListById(Guid id)
        {
            var values = await _getDealerByIdQueryHandler.Handle(new GetDealerByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDealer(CreateDealerCommand command)
        {
            try
            {
                await _createDealerCommandHandler.Handle(command);
                return Ok("Dealer bilgisi başarılı bir şekilde eklendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDealer(UpdateDealerCommand command)
        {
            try
            {
                await _updateDealerCommandHandler.Handle(command);
                return Ok("Dealer bilgisi başarılı bir şekilde güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveDealer(Guid id)
        {
            await _deleteDealerCommandHandler.Handle(new RemoveDealerCommand(id));
            return Ok("Dealer bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteDealer(Guid id)
        {
            await _softDeleteDealerCommandHandler.Handle(new SoftDeleteDealerCommand(id));
            return Ok("Dealer bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreDealer(Guid id)
        {
            await _restoreDealerCommandHandler.Handle(new RestoreDealerCommand(id));
            return Ok("Dealer bilgisi başarılı bir şekilde geri alındı (Restore).");
        }
    }
}

