using MediatR;
using Store.Order.Application.Features.Mediator.Commands.OrderingCommands;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using System;
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
                // Güvenlik K7: Sipariş başka bir kullanıcıya devredilemez (Admin dahil)
                if (request.UserId != Guid.Empty && request.UserId != values.UserId)
                {
                    throw new InvalidOperationException("Sipariş başka bir kullanıcıya devredilemez.");
                }

                // İ3: Durum değişikliği yalnızca durum makinesi üzerinden yapılır
                if (request.Status != values.Status)
                {
                    values.ChangeStatus(request.Status);
                }

                values.OrderDate = request.OrderDate;
                values.TotalPrice = request.TotalPrice;
                await _repository.UpdateAsync(values);
            }
        }
    }
}
