using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands;
using Store.Location.Application.Features.CQRS.Handlers.ContactInfoHandlers;
using Store.Location.Application.Features.CQRS.Queries.ContactInfoQueries;

namespace Store.Location.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactInfoController : ControllerBase
    {
        private readonly ILogger<ContactInfoController> _logger;
        private readonly GetContactInfosQueryHandler _getContactInfosQueryHandler;
        private readonly GetContactInfoByIdQueryHandler _getContactInfoByIdQueryHandler;
        private readonly CreateContactInfoCommandHandler _createContactInfoCommandHandler;
        private readonly UpdateContactInfoCommandHandler _updateContactInfoCommandHandler;
        private readonly RemoveContactInfoCommandHandler _deleteContactInfoCommandHandler;
        private readonly SoftDeleteContactInfoCommandHandler _softDeleteContactInfoCommandHandler;

        public ContactInfoController(
            ILogger<ContactInfoController> logger,
            GetContactInfosQueryHandler getContactInfosQueryHandler,
            GetContactInfoByIdQueryHandler getContactInfoByIdQueryHandler,
            CreateContactInfoCommandHandler createContactInfoCommandHandler,
            UpdateContactInfoCommandHandler updateContactInfoCommandHandler,
            RemoveContactInfoCommandHandler deleteContactInfoCommandHandler,
            SoftDeleteContactInfoCommandHandler softDeleteContactInfoCommandHandler)
        {
            _logger = logger;
            _getContactInfosQueryHandler = getContactInfosQueryHandler;
            _getContactInfoByIdQueryHandler = getContactInfoByIdQueryHandler;
            _createContactInfoCommandHandler = createContactInfoCommandHandler;
            _updateContactInfoCommandHandler = updateContactInfoCommandHandler;
            _deleteContactInfoCommandHandler = deleteContactInfoCommandHandler;
            _softDeleteContactInfoCommandHandler = softDeleteContactInfoCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> ContactInfoList([FromQuery] bool showDeleted = false)
        {
            var values = await _getContactInfosQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ContactInfoListById(Guid id)
        {
            var values = await _getContactInfoByIdQueryHandler.Handle(new GetContactInfoByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContactInfo(CreateContactInfoCommand command)
        {
            await _createContactInfoCommandHandler.Handle(command);
            return Ok("ContactInfo bilgisi başarılı bir şekilde eklendi");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateContactInfo(UpdateContactInfoCommand command)
        {
            await _updateContactInfoCommandHandler.Handle(command);
            return Ok("ContactInfo bilgisi başarılı bir şekilde güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveContactInfo(Guid id)
        {
            await _deleteContactInfoCommandHandler.Handle(new RemoveContactInfoCommand(id));
            return Ok("ContactInfo bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteContactInfo(Guid id)
        {
            await _softDeleteContactInfoCommandHandler.Handle(new SoftDeleteContactInfoCommand(id));
            return Ok("ContactInfo bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }
    }
}

