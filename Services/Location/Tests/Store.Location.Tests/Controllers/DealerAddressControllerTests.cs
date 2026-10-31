using FluentAssertions;
using Store.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Store.Shared.Enums;
using Microsoft.Extensions.Logging;
using Store.Shared.Enums;
using Moq;
using Store.Shared.Enums;
using Store.Location.Application.Features.CQRS.Commands.DealerAddressCommands;
using Store.Shared.Enums;
using Store.Location.Application.Features.CQRS.Handlers.DealerAddressHandlers;
using Store.Shared.Enums;
using Store.Location.Application.Features.CQRS.Queries.DealerAddressQueries;
using Store.Shared.Enums;
using Store.Location.Application.Features.CQRS.Results.DealerAddressResults;
using Store.Shared.Enums;
using Store.Location.Application.Interfaces;
using Store.Shared.Enums;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;
using Store.Location.WebApi.Controllers;
using Store.Shared.Enums;
using System;
using Store.Shared.Enums;
using System.Collections.Generic;
using Store.Shared.Enums;
using System.Threading.Tasks;
using Store.Shared.Enums;
using Xunit;
using Store.Shared.Enums;

namespace Store.Location.Tests.Controllers
{
    public class DealerAddressControllerTests
    {
        private readonly Mock<ILogger<DealerAddressController>> _loggerMock;
        private readonly Mock<IRepository<DealerAddress>> _repositoryMock;

        public DealerAddressControllerTests()
        {
            _loggerMock = new Mock<ILogger<DealerAddressController>>();
            _repositoryMock = new Mock<IRepository<DealerAddress>>();
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenDealerAddressExists()
        {
            // Arrange
            var addressId = Guid.NewGuid();
            var dealerId = Guid.NewGuid();
            var address = new DealerAddress
            {
                Id = addressId,
                DealerId = dealerId,
                Title = "Merkez Ofis",
                AddressType = AddressType.Headquarters,
                ContactFullName = "Ahmet Yılmaz",
                PhoneNumber = "0232 111 22 33",
                AddressDetail = "Atatürk Cad. No: 100",
                CountryId = Guid.NewGuid(),
                CityId = Guid.NewGuid(),
                DistrictId = Guid.NewGuid(),
                IsActive = true
            };

            _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<DealerAddress, bool>>>()))
                           .ReturnsAsync(address);

            var getByIdHandler = new GetDealerAddressByIdQueryHandler(_repositoryMock.Object);
            var getByDealerIdHandler = new GetDealerAddressesByDealerIdQueryHandler(_repositoryMock.Object);
            var createHandler = new CreateDealerAddressCommandHandler(_repositoryMock.Object);
            var updateHandler = new UpdateDealerAddressCommandHandler(_repositoryMock.Object);

            var controller = new DealerAddressController(
                _loggerMock.Object,
                getByIdHandler,
                getByDealerIdHandler,
                createHandler,
                updateHandler
            );

            // Act
            var actionResult = await controller.GetById(addressId);

            // Assert
            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeOfType<GetDealerAddressByIdQueryResult>().Subject;

            result.Id.Should().Be(addressId);
            result.DealerId.Should().Be(dealerId);
            result.Title.Should().Be("Merkez Ofis");
            result.AddressType.Should().Be(AddressType.Headquarters);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenDealerAddressDoesNotExist()
        {
            // Arrange
            var addressId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<DealerAddress, bool>>>()))
                           .ReturnsAsync(default(DealerAddress)!);

            var getByIdHandler = new GetDealerAddressByIdQueryHandler(_repositoryMock.Object);
            var getByDealerIdHandler = new GetDealerAddressesByDealerIdQueryHandler(_repositoryMock.Object);
            var createHandler = new CreateDealerAddressCommandHandler(_repositoryMock.Object);
            var updateHandler = new UpdateDealerAddressCommandHandler(_repositoryMock.Object);

            var controller = new DealerAddressController(
                _loggerMock.Object,
                getByIdHandler,
                getByDealerIdHandler,
                createHandler,
                updateHandler
            );

            // Act
            var actionResult = await controller.GetById(addressId);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetByDealerId_ShouldReturnListOfAddresses_ForDealer()
        {
            // Arrange
            var dealerId = Guid.NewGuid();
            var addressList = new List<DealerAddress>
            {
                new DealerAddress
                {
                    Id = Guid.NewGuid(),
                    DealerId = dealerId,
                    Title = "Merkez Ofis",
                    AddressType = AddressType.Headquarters,
                    ContactFullName = "Ali Kaya",
                    PhoneNumber = "0232 444 55 66",
                    AddressDetail = "Alsancak No: 10",
                    CountryId = Guid.NewGuid(),
                    CityId = Guid.NewGuid(),
                    DistrictId = Guid.NewGuid()
                },
                new DealerAddress
                {
                    Id = Guid.NewGuid(),
                    DealerId = dealerId,
                    Title = "Sevkiyat Deposu",
                    AddressType = AddressType.Shipping,
                    ContactFullName = "Mehmet Demir",
                    PhoneNumber = "0232 777 88 99",
                    AddressDetail = "Pınarbaşı Sanayi No: 5",
                    CountryId = Guid.NewGuid(),
                    CityId = Guid.NewGuid(),
                    DistrictId = Guid.NewGuid()
                }
            };

            _repositoryMock.Setup(r => r.GetAllByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<DealerAddress, bool>>>(), false))
                           .ReturnsAsync(addressList);

            var getByIdHandler = new GetDealerAddressByIdQueryHandler(_repositoryMock.Object);
            var getByDealerIdHandler = new GetDealerAddressesByDealerIdQueryHandler(_repositoryMock.Object);
            var createHandler = new CreateDealerAddressCommandHandler(_repositoryMock.Object);
            var updateHandler = new UpdateDealerAddressCommandHandler(_repositoryMock.Object);

            var controller = new DealerAddressController(
                _loggerMock.Object,
                getByIdHandler,
                getByDealerIdHandler,
                createHandler,
                updateHandler
            );

            // Act
            var actionResult = await controller.GetByDealerId(dealerId);

            // Assert
            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var resultList = okResult.Value.Should().BeAssignableTo<List<GetDealerAddressQueryResult>>().Subject;

            resultList.Should().HaveCount(2);
            resultList[0].AddressType.Should().Be(AddressType.Headquarters);
            resultList[1].AddressType.Should().Be(AddressType.Shipping);
        }

        [Fact]
        public async Task Create_ShouldInvokeCreateAsync_AndReturnOk()
        {
            // Arrange
            var command = new CreateDealerAddressCommand
            {
                DealerId = Guid.NewGuid(),
                AddressType = AddressType.Billing,
                Title = "Fatura Adresi",
                ContactFullName = "Selin Arslan",
                PhoneNumber = "0212 333 44 55",
                AddressDetail = "Maslak Mah. No: 12",
                CountryId = Guid.NewGuid(),
                CityId = Guid.NewGuid(),
                DistrictId = Guid.NewGuid()
            };

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<DealerAddress>()))
                           .Returns(Task.CompletedTask);

            var getByIdHandler = new GetDealerAddressByIdQueryHandler(_repositoryMock.Object);
            var getByDealerIdHandler = new GetDealerAddressesByDealerIdQueryHandler(_repositoryMock.Object);
            var createHandler = new CreateDealerAddressCommandHandler(_repositoryMock.Object);
            var updateHandler = new UpdateDealerAddressCommandHandler(_repositoryMock.Object);

            var controller = new DealerAddressController(
                _loggerMock.Object,
                getByIdHandler,
                getByDealerIdHandler,
                createHandler,
                updateHandler
            );

            // Act
            var actionResult = await controller.Create(command);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            _repositoryMock.Verify(r => r.CreateAsync(It.Is<DealerAddress>(a =>
                a.DealerId == command.DealerId &&
                a.Title == command.Title &&
                a.AddressType == AddressType.Billing &&
                a.ContactFullName == command.ContactFullName
            )), Times.Once);
        }
    }
}
