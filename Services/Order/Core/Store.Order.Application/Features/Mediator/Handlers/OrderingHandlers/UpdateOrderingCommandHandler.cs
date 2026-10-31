using MediatR;
using Store.Order.Application.Features.Mediator.Commands.OrderingCommands;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.Mediator.Handlers.OrderingHandlers
{
    public class UpdateOrderingCommandHandler : IRequestHandler<UpdateOrderingCommand>
    {
        private readonly IRepository<Ordering> _repository;

        public UpdateOrderingCommandHandler(IRepository<Ordering> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateOrderingCommand request, CancellationToken cancellationToken)
        {
            var values = await _repository.GetByIdAsync(request.Id);
            if (values != null)
            {
                values.OrderDate = request.OrderDate;
                values.TotalPrice = request.TotalPrice;
                values.UserId = request.UserId;
                values.Status = request.Status;
                await _repository.UpdateAsync(values);
            }
        }
    }
}
