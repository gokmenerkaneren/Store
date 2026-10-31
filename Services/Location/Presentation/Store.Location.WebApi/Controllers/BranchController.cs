using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Location.Application.Features.CQRS.Commands.BranchCommands;
using Store.Location.Application.Features.CQRS.Handlers.BranchHandlers;
using Store.Location.Application.Features.CQRS.Queries.BranchQueries;

namespace Store.Location.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly ILogger<BranchController> _logger;
        private readonly GetBranchQueryHandler _getBranchesQueryHandler;
        private readonly GetBranchByIdQueryHandler _getBranchByIdQueryHandler;
        private readonly CreateBranchCommandHandler _createBranchCommandHandler;
        private readonly UpdateBranchCommandHandler _updateBranchCommandHandler;
        private readonly RemoveBranchCommandHandler _deleteBranchCommandHandler;
        private readonly SoftDeleteBranchCommandHandler _softDeleteBranchCommandHandler;
        private readonly RestoreBranchCommandHandler _restoreBranchCommandHandler;

        public BranchController(
            ILogger<BranchController> logger,
            GetBranchQueryHandler getBranchesQueryHandler,
            GetBranchByIdQueryHandler getBranchByIdQueryHandler,
            CreateBranchCommandHandler createBranchCommandHandler,
            UpdateBranchCommandHandler updateBranchCommandHandler,
            RemoveBranchCommandHandler deleteBranchCommandHandler,
            SoftDeleteBranchCommandHandler softDeleteBranchCommandHandler,
            RestoreBranchCommandHandler restoreBranchCommandHandler)
        {
            _logger = logger;
            _getBranchesQueryHandler = getBranchesQueryHandler;
            _getBranchByIdQueryHandler = getBranchByIdQueryHandler;
            _createBranchCommandHandler = createBranchCommandHandler;
            _updateBranchCommandHandler = updateBranchCommandHandler;
            _deleteBranchCommandHandler = deleteBranchCommandHandler;
            _softDeleteBranchCommandHandler = softDeleteBranchCommandHandler;
            _restoreBranchCommandHandler = restoreBranchCommandHandler;
        }

        [HttpGet]
        public async Task<IActionResult> BranchList([FromQuery] bool showDeleted = false)
        {
                var values = await _getBranchesQueryHandler.Handle(showDeleted);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BranchListById(Guid id)
        {
            var values = await _getBranchByIdQueryHandler.Handle(new GetBranchByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch(CreateBranchCommand command)
        {
            try
            {
                await _createBranchCommandHandler.Handle(command);
                return Ok("Branch bilgisi başarılı bir şekilde eklendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBranch(UpdateBranchCommand command)
        {
            try
            {
                await _updateBranchCommandHandler.Handle(command);
                return Ok("Branch bilgisi başarılı bir şekilde güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveBranch(Guid id)
        {
            await _deleteBranchCommandHandler.Handle(new RemoveBranchCommand(id));
            return Ok("Branch bilgisi başarılı bir şekilde silindi");
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteBranch(Guid id)
        {
            await _softDeleteBranchCommandHandler.Handle(new SoftDeleteBranchCommand(id));
            return Ok("Branch bilgisi başarılı bir şekilde silinerek arşivlendi (Soft Delete).");
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreBranch(Guid id)
        {
            await _restoreBranchCommandHandler.Handle(new RestoreBranchCommand(id));
            return Ok("Branch bilgisi başarılı bir şekilde geri alındı (Restore).");
        }
    }
}

