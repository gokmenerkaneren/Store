using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers
{
    public class SetDefaultUserAddressCommandHandler
    {
        private readonly IRepository<UserAddress> _repository;

        public SetDefaultUserAddressCommandHandler(IRepository<UserAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SetDefaultUserAddressCommand command)
        {
            var targetAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id && !x.IsDeleted);
            if (targetAddress == null)
            {
                throw new InvalidOperationException("Varsayılan yapılmak istenen adres bulunamadı.");
            }

            var customerId = targetAddress.CustomerId;
            var allAddresses = await _repository.GetAllByFilterAsync(x => x.CustomerId == customerId && !x.IsDeleted);

            foreach (var addr in allAddresses)
            {
                if (addr.Id == targetAddress.Id)
                {
                    addr.IsDefault = true;
                }
                else
                {
                    addr.IsDefault = false;
                }
                addr.UpdateUserId = Guid.Parse("00000002-0002-0002-0000-000000000002");
                addr.UpdateDate = BaseEntity.GetLocalTime();
                await _repository.UpdateAsync(addr);
            }
        }
    }
}

