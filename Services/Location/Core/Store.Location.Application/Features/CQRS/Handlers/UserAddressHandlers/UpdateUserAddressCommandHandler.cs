using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers
{
    public class UpdateUserAddressCommandHandler
    {
        private readonly IRepository<UserAddress> _repository;

        public UpdateUserAddressCommandHandler(IRepository<UserAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateUserAddressCommand command)
        {
            var userAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (userAddress == null) return;

            var trimmedTitle = command.Title?.Trim();
            if (!string.IsNullOrEmpty(trimmedTitle))
            {
                var existing = await _repository.GetByFilterAsync(x =>
                    x.CustomerId == command.CustomerId &&
                    !x.IsDeleted &&
                    x.Id != command.Id &&
                    x.Title.ToLower() == trimmedTitle.ToLower());

                if (existing != null)
                {
                    throw new InvalidOperationException($"'{trimmedTitle}' başlıklı bir adresiniz zaten bulunmaktadır. Lütfen farklı bir adres başlığı belirleyiniz.");
                }
            }

            var otherCustomerAddresses = await _repository.GetAllByFilterAsync(x =>
                x.CustomerId == command.CustomerId &&
                x.Id != command.Id &&
                !x.IsDeleted);

            bool shouldBeDefault = command.IsDefault;
            // Eğer tek adres ise zorunlu olarak varsayılan kalır
            if (otherCustomerAddresses == null || otherCustomerAddresses.Count == 0)
            {
                shouldBeDefault = true;
            }
            else if (shouldBeDefault)
            {
                // Diğer tüm adreslerin varsayılan özelliğini kaldır
                foreach (var other in otherCustomerAddresses.Where(x => x.IsDefault))
                {
                    other.IsDefault = false;
                    other.UpdateDate = BaseEntity.GetLocalTime();
                    await _repository.UpdateAsync(other);
                }
            }

            userAddress.Title = command.Title;
            userAddress.ContactFullName = command.ContactFullName;
            userAddress.PhoneNumber = command.PhoneNumber;
            userAddress.AddressDetail = command.AddressDetail;
            userAddress.CountryId = command.CountryId;
            userAddress.CityId = command.CityId;
            userAddress.DistrictId = command.DistrictId;
            userAddress.NeighborhoodId = command.NeighborhoodId;
            userAddress.CustomerId = command.CustomerId;
            userAddress.InvoiceType = command.InvoiceType;
            userAddress.IdentityNumber = command.IdentityNumber;
            userAddress.TaxOffice = command.TaxOffice;
            userAddress.TaxNumber = command.TaxNumber;
            userAddress.CompanyId = command.CompanyId;
            userAddress.IsActive = command.IsActive;
            userAddress.IsDefault = shouldBeDefault;
            userAddress.Notes = command.Notes;
            userAddress.UpdateUserId = Guid.Parse("00000002-0002-0002-0000-000000000002");
            userAddress.UpdateDate = BaseEntity.GetLocalTime();

            await _repository.UpdateAsync(userAddress);
        }
    }
}
