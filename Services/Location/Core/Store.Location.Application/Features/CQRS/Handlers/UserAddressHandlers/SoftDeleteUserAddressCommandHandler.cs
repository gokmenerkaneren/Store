using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers
{
    public class SoftDeleteUserAddressCommandHandler
    {
        private readonly IRepository<UserAddress> _repository;

        public SoftDeleteUserAddressCommandHandler(IRepository<UserAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteUserAddressCommand command)
        {
            var userAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (userAddress == null) return;

            bool wasDefault = userAddress.IsDefault;
            var customerId = userAddress.CustomerId;

            await _repository.SoftDeleteAsync(userAddress);

            // Eğer silinen adres varsayılan ise, kalan adreslerden en günceli otomatik varsayılan yapılır
            if (wasDefault)
            {
                var remaining = await _repository.GetAllByFilterAsync(x => x.CustomerId == customerId && !x.IsDeleted);
                if (remaining != null && remaining.Any())
                {
                    var newDefault = remaining.OrderByDescending(x => x.InsertDate).First();
                    newDefault.IsDefault = true;
                    newDefault.UpdateDate = BaseEntity.GetLocalTime();
                    await _repository.UpdateAsync(newDefault);
                }
            }
        }
    }
}
