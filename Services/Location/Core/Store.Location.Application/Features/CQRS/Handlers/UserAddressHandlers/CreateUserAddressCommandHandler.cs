using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers
{
    public class CreateUserAddressCommandHandler
    {
        private readonly IRepository<UserAddress> _repository;

        public CreateUserAddressCommandHandler(IRepository<UserAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateUserAddressCommand command)
        {
            var trimmedTitle = command.Title?.Trim();
            if (!string.IsNullOrEmpty(trimmedTitle))
            {
                var existing = await _repository.GetByFilterAsync(x =>
                    x.CustomerId == command.CustomerId &&
                    !x.IsDeleted &&
                    x.Title.ToLower() == trimmedTitle.ToLower());

                if (existing != null)
                {
                    throw new InvalidOperationException($"'{trimmedTitle}' başlıklı bir adresiniz zaten bulunmaktadır. Lütfen farklı bir adres başlığı belirleyiniz.");
                }
            }

            // Müşterinin mevcut aktif adresleri kontrol edilir
            var existingCustomerAddresses = await _repository.GetAllByFilterAsync(x => x.CustomerId == command.CustomerId && !x.IsDeleted);
            
            bool shouldBeDefault = command.IsDefault;
            // İlk adres ise direkt olarak varsayılan teslimat adresi kabul edilir
            if (existingCustomerAddresses == null || existingCustomerAddresses.Count == 0)
            {
                shouldBeDefault = true;
            }
            else if (shouldBeDefault)
            {
                // Eğer yeni adres varsayılan seçildiyse, diğer tüm adresler varsayılan olmaktan çıkarılır
                foreach (var addr in existingCustomerAddresses.Where(x => x.IsDefault))
                {
                    addr.IsDefault = false;
                    addr.UpdateDate = BaseEntity.GetLocalTime();
                    await _repository.UpdateAsync(addr);
                }
            }

            var userAddress = new UserAddress
            {
                Title = command.Title,
                ContactFullName = command.ContactFullName,
                PhoneNumber = command.PhoneNumber,
                AddressDetail = command.AddressDetail,
                CountryId = command.CountryId,
                CityId = command.CityId,
                DistrictId = command.DistrictId,
                NeighborhoodId = command.NeighborhoodId,
                CustomerId = command.CustomerId,
                InvoiceType = command.InvoiceType,
                IdentityNumber = command.IdentityNumber,
                TaxOffice = command.TaxOffice,
                TaxNumber = command.TaxNumber,
                CompanyId = command.CompanyId,
                IsAnonymized = false,
                Notes = command.Notes,
                IsActive = true,
                IsDefault = shouldBeDefault,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(userAddress);
        }
    }
}
