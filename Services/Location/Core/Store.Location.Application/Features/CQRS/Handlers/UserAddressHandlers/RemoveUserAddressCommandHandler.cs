using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers
{
    public class RemoveUserAddressCommandHandler
    {
        private readonly IRepository<UserAddress> _repository;

        public RemoveUserAddressCommandHandler(IRepository<UserAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveUserAddressCommand command)
        {
            var userAddress = await _repository.GetByIdAsync(command.Id);
            if (userAddress != null)
            {
                await _repository.SoftDeleteAsync(userAddress);
            }
        }
    }
}
