using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Store.Rental.Application.Features.RentalOrders.Commands.CreateRentalOrder;
using Store.Rental.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Store.Rental.Tests
{
    public class RentalsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RentalsController _controller;

        public RentalsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new RentalsController(_mediatorMock.Object);
        }

        private void SetUserContext(Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("sub", userId.ToString()),
                new Claim("role", "Customer")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task CreateRentalOrder_Should_Bind_UserId_From_Token_Claims_And_Ignore_Body_UserId()
        {
            // Arrange: Kullanıcı A oturum açmış, fakat request body'sinde başka bir UserId yollamış
            var loggedInUserId = Guid.NewGuid();
            var attackerUserId = Guid.NewGuid();
            SetUserContext(loggedInUserId);

            var command = new CreateRentalOrderCommand
            {
                UserId = attackerUserId,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(4),
                Items = new List<RentalOrderItemDto>
                {
                    new RentalOrderItemDto { ProductId = Guid.NewGuid().ToString(), Quantity = 1, UnitPrice = 100 }
                }
            };

            CreateRentalOrderCommand capturedCommand = null;
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateRentalOrderCommand>(), It.IsAny<CancellationToken>()))
                         .Callback<IRequest<Guid>, CancellationToken>((req, ct) => capturedCommand = req as CreateRentalOrderCommand)
                         .ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _controller.CreateRentalOrder(command);

            // Assert: Body'deki sahte UserId ezilmiş ve token'daki gerçek UserId atanmış olmalı
            result.Should().BeOfType<OkObjectResult>();
            capturedCommand.Should().NotBeNull();
            capturedCommand.UserId.Should().Be(loggedInUserId);
            capturedCommand.UserId.Should().NotBe(attackerUserId);
        }

        [Fact]
        public async Task CreateRentalOrder_Should_Return_Unauthorized_When_User_Not_Authenticated()
        {
            // Arrange: Anonim istek
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var command = new CreateRentalOrderCommand
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                Items = new List<RentalOrderItemDto>
                {
                    new RentalOrderItemDto { ProductId = "test", Quantity = 1, UnitPrice = 50 }
                }
            };

            // Act
            var result = await _controller.CreateRentalOrder(command);

            // Assert: Unauthorized dönmeli
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}
