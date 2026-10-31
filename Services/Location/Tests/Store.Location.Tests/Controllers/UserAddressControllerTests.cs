using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Store.Location.Application.Features.CQRS.Commands.UserAddressCommands;
using Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers;
using Store.Location.Application.Features.CQRS.Queries.UserAddressQueries;
using Store.Location.Application.Features.CQRS.Results.UserAddressResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using Store.Location.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Store.Location.Tests.Controllers
{
    public class UserAddressControllerTests
    {
        private readonly Mock<ILogger<UserAddressController>> _loggerMock;
        private readonly Mock<IRepository<UserAddress>> _repositoryMock;
        private readonly GetUserAddressesQueryHandler _getUserAddressesQueryHandler;
        private readonly GetUserAddressByIdQueryHandler _getUserAddressByIdQueryHandler;
        private readonly CreateUserAddressCommandHandler _createUserAddressCommandHandler;
        private readonly UpdateUserAddressCommandHandler _updateUserAddressCommandHandler;
        private readonly RemoveUserAddressCommandHandler _deleteUserAddressCommandHandler;
        private readonly SoftDeleteUserAddressCommandHandler _softDeleteUserAddressCommandHandler;
        private readonly AnonymizeUserAddressCommandHandler _anonymizeUserAddressCommandHandler;
        private readonly SetDefaultUserAddressCommandHandler _setDefaultUserAddressCommandHandler;

        public UserAddressControllerTests()
        {
            _loggerMock = new Mock<ILogger<UserAddressController>>();
            _repositoryMock = new Mock<IRepository<UserAddress>>();

            _getUserAddressesQueryHandler = new GetUserAddressesQueryHandler(_repositoryMock.Object);
            _getUserAddressByIdQueryHandler = new GetUserAddressByIdQueryHandler(_repositoryMock.Object);
            _createUserAddressCommandHandler = new CreateUserAddressCommandHandler(_repositoryMock.Object);
            _updateUserAddressCommandHandler = new UpdateUserAddressCommandHandler(_repositoryMock.Object);
            _deleteUserAddressCommandHandler = new RemoveUserAddressCommandHandler(_repositoryMock.Object);
            _softDeleteUserAddressCommandHandler = new SoftDeleteUserAddressCommandHandler(_repositoryMock.Object);
            _anonymizeUserAddressCommandHandler = new AnonymizeUserAddressCommandHandler(_repositoryMock.Object);
            _setDefaultUserAddressCommandHandler = new SetDefaultUserAddressCommandHandler(_repositoryMock.Object);
        }

        private UserAddressController CreateControllerWithUser(Guid? userId, string role = "Customer")
        {
            var controller = new UserAddressController(
                _loggerMock.Object,
                _getUserAddressesQueryHandler,
                _getUserAddressByIdQueryHandler,
                _createUserAddressCommandHandler,
                _updateUserAddressCommandHandler,
                _deleteUserAddressCommandHandler,
                _softDeleteUserAddressCommandHandler,
                _anonymizeUserAddressCommandHandler,
                _setDefaultUserAddressCommandHandler);

            var claims = new List<Claim>();
            if (userId.HasValue)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = userId.HasValue ? new ClaimsIdentity(claims, "TestAuth") : new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        [Fact]
        public async Task UserAddressList_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var controller = CreateControllerWithUser(null);

            // Act
            var result = await controller.UserAddressList();

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task UserAddressList_ShouldOnlyReturnCurrentUserAddresses_IDOR_Protection()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var addresses = new List<UserAddress>
            {
                new UserAddress { Id = Guid.NewGuid(), CustomerId = currentUserId, Title = "Evim", IsDeleted = false },
                new UserAddress { Id = Guid.NewGuid(), CustomerId = otherUserId, Title = "Başkası", IsDeleted = false }
            };

            _repositoryMock.Setup(r => r.GetAllAsync(false)).ReturnsAsync(addresses);

            var controller = CreateControllerWithUser(currentUserId);

            // Act
            var result = await controller.UserAddressList();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var items = okResult.Value.Should().BeAssignableTo<List<GetUserAddressQueryResult>>().Subject;

            items.Should().HaveCount(1);
            items[0].CustomerId.Should().Be(currentUserId);
            items[0].Title.Should().Be("Evim");
        }

        [Fact]
        public async Task UserAddressListById_ShouldReturnForbid_WhenAccessingOtherUserAddress_IDOR_Protection()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var addressId = Guid.NewGuid();

            var otherAddress = new UserAddress
            {
                Id = addressId,
                CustomerId = otherUserId,
                Title = "Gizli Adres",
                IsDeleted = false
            };

            _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserAddress, bool>>>()))
                .ReturnsAsync(otherAddress);

            var controller = CreateControllerWithUser(currentUserId);

            // Act
            var result = await controller.UserAddressListById(addressId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task CreateUserAddress_ShouldEnforceCustomerIdFromToken_IDOR_Protection()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var maliciousCustomerId = Guid.NewGuid();

            var command = new CreateUserAddressCommand
            {
                CustomerId = maliciousCustomerId, // Kötü niyetli istemcinin gönderdiği başkasının kimliği
                Title = "Yeni Adres"
            };

            UserAddress savedEntity = null;
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserAddress>()))
                .Callback<UserAddress>(e => savedEntity = e)
                .Returns(Task.CompletedTask);

            var controller = CreateControllerWithUser(currentUserId);

            // Act
            var result = await controller.CreateUserAddress(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            command.CustomerId.Should().Be(currentUserId); // Token'daki kimlik zorlanmalı
        }
    }
}
