using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers
{
    public class AnonymizeUserAddressCommandHandler
    {
        private readonly IRepository<UserAddress> _repository;

        public AnonymizeUserAddressCommandHandler(IRepository<UserAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(AnonymizeUserAddressCommand command)
        {
            var userAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (userAddress == null) return;

            userAddress.AnonymizeData();
            userAddress.UpdateDate = BaseEntity.GetLocalTime();

            await _repository.UpdateAsync(userAddress);
        }
    }
}

