using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.DealerAddressCommands;
using Store.Location.Application.Features.CQRS.Handlers.DealerAddressHandlers;
using Store.Location.Application.Features.CQRS.Queries.DealerAddressQueries;

namespace Store.Location.WebApi.Controllers
{
    /// <summary>
    /// Bayi (Dealer) fiziksel adres yönetimi.
    /// Bir bayinin birden fazla adresi olabilir: Merkez (1), Fatura (2), Sevkiyat (3).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DealerAddressController : ControllerBase
    {
        private readonly ILogger<DealerAddressController> _logger;
        private readonly GetDealerAddressByIdQueryHandler _getByIdHandler;
        private readonly GetDealerAddressesByDealerIdQueryHandler _getByDealerIdHandler;
        private readonly CreateDealerAddressCommandHandler _createHandler;
        private readonly UpdateDealerAddressCommandHandler _updateHandler;

        public DealerAddressController(
            ILogger<DealerAddressController> logger,
            GetDealerAddressByIdQueryHandler getByIdHandler,
            GetDealerAddressesByDealerIdQueryHandler getByDealerIdHandler,
            CreateDealerAddressCommandHandler createHandler,
            UpdateDealerAddressCommandHandler updateHandler)
        {
            _logger = logger;
            _getByIdHandler = getByIdHandler;
            _getByDealerIdHandler = getByDealerIdHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
        }

        /// <summary>
        /// Belirli bir ID'ye sahip bayi adresini getirir.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _getByIdHandler.Handle(new GetDealerAddressByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Belirli bir bayiye ait tüm adresleri getirir (Merkez, Fatura, Sevkiyat).
        /// </summary>
        [HttpGet("by-dealer/{dealerId}")]
        public async Task<IActionResult> GetByDealerId(Guid dealerId)
        {
            var result = await _getByDealerIdHandler.Handle(new GetDealerAddressesByDealerIdQuery(dealerId));
            return Ok(result);
        }

        /// <summary>
        /// Bayiye yeni bir adres ekler.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateDealerAddressCommand command)
        {
            await _createHandler.Handle(command);
            return Ok("Bayi adresi başarıyla eklendi.");
        }

        /// <summary>
        /// Mevcut bir bayi adresini günceller.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update(UpdateDealerAddressCommand command)
        {
            await _updateHandler.Handle(command);
            return Ok("Bayi adresi başarıyla güncellendi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveDealerAddress(Guid id)
        {
            await HttpContext.RequestServices.GetRequiredService<DeleteDealerAddressCommandHandler>().Handle(new DeleteDealerAddressCommand(id));
            return Ok("Bayi adresi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteDealerAddress(Guid id)
        {
            await HttpContext.RequestServices.GetRequiredService<SoftDeleteDealerAddressCommandHandler>().Handle(new SoftDeleteDealerAddressCommand(id));
            return Ok("Bayi adresi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreDealerAddress(Guid id)
        {
            await HttpContext.RequestServices.GetRequiredService<RestoreDealerAddressCommandHandler>().Handle(new RestoreDealerAddressCommand(id));
            return Ok("Bayi adresi başarılı bir şekilde geri alındı (Restore).");
        }
    }
}
