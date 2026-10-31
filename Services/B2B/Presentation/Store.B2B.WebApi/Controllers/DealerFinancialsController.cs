using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.B2B.Application.Features.CQRS.Commands.DealerFinancialCommands;
using Store.B2B.Application.Features.CQRS.Handlers.DealerFinancialHandlers;
using Store.B2B.Application.Features.CQRS.Queries.DealerFinancialQueries;

namespace Store.B2B.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DealerFinancialsController : ControllerBase
    {
        private readonly GetAllDealerFinancialQueryHandler _getAllHandler;
        private readonly GetDealerFinancialByIdQueryHandler _getByIdHandler;
        private readonly CreateDealerFinancialCommandHandler _createHandler;
        private readonly UpdateDealerFinancialCommandHandler _updateHandler;
        private readonly RemoveDealerFinancialCommandHandler _removeHandler;

        public DealerFinancialsController(
            GetAllDealerFinancialQueryHandler getAllHandler,
            GetDealerFinancialByIdQueryHandler getByIdHandler,
            CreateDealerFinancialCommandHandler createHandler,
            UpdateDealerFinancialCommandHandler updateHandler,
            RemoveDealerFinancialCommandHandler removeHandler)
        {
            _getAllHandler = getAllHandler;
            _getByIdHandler = getByIdHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _removeHandler = removeHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var values = await _getAllHandler.Handle(new GetAllDealerFinancialQuery());
            return Ok(values);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var value = await _getByIdHandler.Handle(new GetDealerFinancialByIdQuery(id));
            if (value is null) return NotFound();
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDealerFinancialCommand command)
        {
            await _createHandler.Handle(command);
            return Ok("Bayi finansal kaydı oluşturuldu.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDealerFinancialCommand command)
        {
            await _updateHandler.Handle(command);
            return Ok("Bayi finansal kaydı güncellendi.");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _removeHandler.Handle(new RemoveDealerFinancialCommand(id));
            return Ok("Bayi finansal kaydı silindi.");
        }
    }
}
