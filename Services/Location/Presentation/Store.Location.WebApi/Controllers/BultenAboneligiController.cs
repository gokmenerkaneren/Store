using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.BultenAboneligiCommands;
using Store.Location.Application.Features.CQRS.Handlers.BultenAboneligiHandlers;
using Store.Location.Application.Features.CQRS.Queries.BultenAboneligiQueries;
using System;
using System.Threading.Tasks;

namespace Store.Location.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BultenAboneligiController : ControllerBase
    {
        private readonly GetBultenAboneligiQueryHandler _getQueryHandler;
        private readonly GetBultenAboneligiByIdQueryHandler _getByIdQueryHandler;
        private readonly CreateBultenAboneligiCommandHandler _createCommandHandler;
        private readonly UpdateBultenAboneligiCommandHandler _updateCommandHandler;
        private readonly RemoveBultenAboneligiCommandHandler _removeCommandHandler;

        private readonly CheckBultenAboneligiQueryHandler _checkQueryHandler;

        public BultenAboneligiController(
            GetBultenAboneligiQueryHandler getQueryHandler,
            GetBultenAboneligiByIdQueryHandler getByIdQueryHandler,
            CreateBultenAboneligiCommandHandler createCommandHandler,
            UpdateBultenAboneligiCommandHandler updateCommandHandler,
            RemoveBultenAboneligiCommandHandler removeCommandHandler,
            CheckBultenAboneligiQueryHandler checkQueryHandler)
        {
            _getQueryHandler = getQueryHandler;
            _getByIdQueryHandler = getByIdQueryHandler;
            _createCommandHandler = createCommandHandler;
            _updateCommandHandler = updateCommandHandler;
            _removeCommandHandler = removeCommandHandler;
            _checkQueryHandler = checkQueryHandler;
        }

        [HttpGet]
        public async Task<IActionResult> SubscriberList([FromQuery] bool showDeleted = false)
        {
            var values = await _getQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("Check")]
        public async Task<IActionResult> CheckSubscriber([FromQuery] string email)
        {
            var result = await _checkQueryHandler.Handle(new CheckBultenAboneligiQuery(email));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> SubscriberById(Guid id)
        {
            var values = await _getByIdQueryHandler.Handle(new GetBultenAboneligiByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(CreateBultenAboneligiCommand command)
        {
            await _createCommandHandler.Handle(command);
            return Ok("Abonelik baÅŸarÄ±yla oluÅŸturuldu.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSubscriber(UpdateBultenAboneligiCommand command)
        {
            await _updateCommandHandler.Handle(command);
            return Ok("Abonelik durumu baÅŸarÄ±yla gÃ¼ncellendi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSubscriber(Guid id)
        {
            await _removeCommandHandler.Handle(new RemoveBultenAboneligiCommand(id));
            return Ok("Abonelik baÅŸarÄ±yla silindi.");
        }
    }
}
