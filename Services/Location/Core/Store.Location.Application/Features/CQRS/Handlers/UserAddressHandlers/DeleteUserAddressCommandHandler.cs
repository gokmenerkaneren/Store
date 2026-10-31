using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers
{
    public class DeleteUserAddressCommandHandler
    {
        private readonly IRepository<UserAddress> _repository;

        public DeleteUserAddressCommandHandler(IRepository<UserAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteUserAddressCommand command)
        {
            var userAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (userAddress == null) return;

            await _repository.SoftDeleteAsync(userAddress);
        }
    }
}
