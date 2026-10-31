using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Store.Location.Application.Features.CQRS.Commands.WarehouseCommands;
using Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers;
using Store.Location.Application.Features.CQRS.Queries.WarehouseQueries;
using Store.Location.Application.Features.CQRS.Results.WarehouseResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using Store.Location.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Store.Location.Tests.Controllers
{
    public class WarehouseControllerTests
    {
        private readonly Mock<ILogger<WarehouseController>> _loggerMock;
        private readonly Mock<IRepository<Warehouse>> _repositoryMock;

        public WarehouseControllerTests()
        {
            _loggerMock = new Mock<ILogger<WarehouseController>>();
            _repositoryMock = new Mock<IRepository<Warehouse>>();
        }

        [Fact]
        public async Task WarehouseList_ShouldReturnAllWarehouses()
        {
            // Arrange
            var contactInfoId = Guid.NewGuid();
            var warehouseList = new List<Warehouse>
            {
                new Warehouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Ana Lojistik Deposu",
                    Code = "WH-01",
                    IsDefault = true,
                    ContactInfoId = contactInfoId,
                    IsActive = true,
                    IsDeleted = false
                },
                new Warehouse
                {
                    Id = Guid.NewGuid(),
                    Name = "Ege Bölge Deposu",
                    Code = "WH-02",
                    IsDefault = false,
                    ContactInfoId = contactInfoId,
                    IsActive = true,
                    IsDeleted = false
                }
            };

            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<bool>()))
                           .ReturnsAsync(warehouseList);

            var mockAddressRepository = new Mock<IRepository<WarehouseAddress>>();
            mockAddressRepository.Setup(r => r.GetAllAsync(It.IsAny<bool>()))
                                 .ReturnsAsync(new List<WarehouseAddress>());

            var getWarehousesHandler = new GetWarehousesQueryHandler(_repositoryMock.Object, mockAddressRepository.Object);
            var getByIdHandler = new GetWarehouseByIdQueryHandler(_repositoryMock.Object, mockAddressRepository.Object);
            var createHandler = new CreateWarehouseCommandHandler(_repositoryMock.Object, mockAddressRepository.Object);
            var updateHandler = new UpdateWarehouseCommandHandler(_repositoryMock.Object, mockAddressRepository.Object);
            var removeHandler = new RemoveWarehouseCommandHandler(_repositoryMock.Object);
            var softDeleteHandler = new SoftDeleteWarehouseCommandHandler(_repositoryMock.Object);
            var restoreHandler = new RestoreWarehouseCommandHandler(_repositoryMock.Object);

            var controller = new WarehouseController(
                _loggerMock.Object,
                getWarehousesHandler,
                getByIdHandler,
                createHandler,
                updateHandler,
                removeHandler,
                softDeleteHandler,
                restoreHandler
            );

            // Act
            var actionResult = await controller.WarehouseList(showDeleted: false);

            // Assert
            var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
            var resultList = okResult.Value.Should().BeAssignableTo<List<GetWarehouseQueryResult>>().Subject;

            resultList.Should().HaveCount(2);
            resultList[0].Name.Should().Be("Ana Lojistik Deposu");
            resultList[0].ContactInfoId.Should().Be(contactInfoId);
        }

        [Fact]
        public async Task CreateWarehouse_ShouldInvokeCreateAsyncWithContactInfoId()
        {
            // Arrange
            var contactInfoId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var command = new CreateWarehouseCommand
            {
                Name = "Kuzey Deposu",
                Code = "WH-03",
                IsDefault = false,
                BranchId = branchId,
                ContactInfoId = contactInfoId,
                Notes = "Yeni yapılanan depo"
            };

            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<bool>()))
                           .ReturnsAsync(new List<Warehouse>());
            _repositoryMock.Setup(r => r.GetAllByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Warehouse, bool>>>(), It.IsAny<bool>()))
                           .ReturnsAsync(new List<Warehouse>());
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Warehouse>()))
                           .Returns(Task.CompletedTask);

            var getWarehousesHandler = new GetWarehousesQueryHandler(_repositoryMock.Object, new Mock<IRepository<WarehouseAddress>>().Object);
            var getByIdHandler = new GetWarehouseByIdQueryHandler(_repositoryMock.Object, new Mock<IRepository<WarehouseAddress>>().Object);
            var createHandler = new CreateWarehouseCommandHandler(_repositoryMock.Object, new Mock<IRepository<WarehouseAddress>>().Object);
            var updateHandler = new UpdateWarehouseCommandHandler(_repositoryMock.Object, new Mock<IRepository<WarehouseAddress>>().Object);
            var removeHandler = new RemoveWarehouseCommandHandler(_repositoryMock.Object);
            var softDeleteHandler = new SoftDeleteWarehouseCommandHandler(_repositoryMock.Object);
            var restoreHandler = new RestoreWarehouseCommandHandler(_repositoryMock.Object);

            var controller = new WarehouseController(
                _loggerMock.Object,
                getWarehousesHandler,
                getByIdHandler,
                createHandler,
                updateHandler,
                removeHandler,
                softDeleteHandler,
                restoreHandler
            );

            // Act
            var actionResult = await controller.CreateWarehouse(command);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            _repositoryMock.Verify(r => r.CreateAsync(It.Is<Warehouse>(w =>
                w.Name == command.Name &&
                w.Code == command.Code &&
                w.BranchId == branchId &&
                w.ContactInfoId == contactInfoId
            )), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteWarehouse_ShouldInvokeSoftDeleteAsync()
        {
            // Arrange
            var warehouseId = Guid.NewGuid();
            var warehouse = new Warehouse { Id = warehouseId, Name = "Silinecek Depo" };

            _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Warehouse, bool>>>()))
                           .ReturnsAsync(warehouse);
            _repositoryMock.Setup(r => r.SoftDeleteAsync(It.IsAny<Warehouse>()))
                           .Returns(Task.CompletedTask);

            var getWarehousesHandler = new GetWarehousesQueryHandler(_repositoryMock.Object, new Mock<IRepository<WarehouseAddress>>().Object);
            var getByIdHandler = new GetWarehouseByIdQueryHandler(_repositoryMock.Object, new Mock<IRepository<WarehouseAddress>>().Object);
            var createHandler = new CreateWarehouseCommandHandler(_repositoryMock.Object, new Mock<IRepository<WarehouseAddress>>().Object);
            var updateHandler = new UpdateWarehouseCommandHandler(_repositoryMock.Object, new Mock<IRepository<WarehouseAddress>>().Object);
            var removeHandler = new RemoveWarehouseCommandHandler(_repositoryMock.Object);
            var softDeleteHandler = new SoftDeleteWarehouseCommandHandler(_repositoryMock.Object);
            var restoreHandler = new RestoreWarehouseCommandHandler(_repositoryMock.Object);

            var controller = new WarehouseController(
                _loggerMock.Object,
                getWarehousesHandler,
                getByIdHandler,
                createHandler,
                updateHandler,
                removeHandler,
                softDeleteHandler,
                restoreHandler
            );

            // Act
            var actionResult = await controller.SoftDeleteWarehouse(warehouseId);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            _repositoryMock.Verify(r => r.SoftDeleteAsync(It.Is<Warehouse>(w => w.Id == warehouseId)), Times.Once);
        }
    }
}

