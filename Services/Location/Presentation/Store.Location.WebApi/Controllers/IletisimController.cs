using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.MesajlarCommands;
using Store.Location.Application.Features.CQRS.Handlers.MesajlarHandlers;
using Store.Location.Application.Features.CQRS.Queries.MesajlarQueries;
using System;
using System.Threading.Tasks;

namespace Store.Location.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IletisimController : ControllerBase
    {
        private readonly GetMesajlarQueryHandler _getQueryHandler;
        private readonly GetMesajlarByIdQueryHandler _getByIdQueryHandler;
        private readonly CreateMesajlarCommandHandler _createCommandHandler;
        private readonly UpdateMesajlarCommandHandler _updateCommandHandler;
        private readonly RemoveMesajlarCommandHandler _removeCommandHandler;

        public IletisimController(
            GetMesajlarQueryHandler getQueryHandler,
            GetMesajlarByIdQueryHandler getByIdQueryHandler,
            CreateMesajlarCommandHandler createCommandHandler,
            UpdateMesajlarCommandHandler updateCommandHandler,
            RemoveMesajlarCommandHandler removeCommandHandler)
        {
            _getQueryHandler = getQueryHandler;
            _getByIdQueryHandler = getByIdQueryHandler;
            _createCommandHandler = createCommandHandler;
            _updateCommandHandler = updateCommandHandler;
            _removeCommandHandler = removeCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> MessageList([FromQuery] bool showDeleted = false)
        {
            var values = await _getQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> MessageById(Guid id)
        {
            var values = await _getByIdQueryHandler.Handle(new GetMesajlarByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMesajlarCommand command)
        {
            await _createCommandHandler.Handle(command);
            return Ok("Mesaj baÅŸarÄ±yla gÃ¶nderildi.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMessage(UpdateMesajlarCommand command)
        {
            await _updateCommandHandler.Handle(command);
            return Ok("Mesaj durumu baÅŸarÄ±yla gÃ¼ncellendi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveMessage(Guid id)
        {
            await _removeCommandHandler.Handle(new RemoveMesajlarCommand(id));
            return Ok("Mesaj baÅŸarÄ±yla silindi.");
        }
    }
}
