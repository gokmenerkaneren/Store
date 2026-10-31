using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Store.Order.Application.Features.Mediator.Commands.OrderingCommands;
using Store.Order.Application.Features.Mediator.Queries.OrderingQueries;
using Store.Order.Application.Features.Mediator.Results.OrderingResults;
using Store.Order.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Store.Order.Tests.Controllers
{
    public class OrderingsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly OrderingsController _controller;

        public OrderingsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new OrderingsController(_mediatorMock.Object);
        }

        private void SetUserContext(Guid userId, string role = "Customer")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("role", role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task CreateOrdering_Should_Bind_UserId_From_Token_Claims_And_Ignore_Payload_UserId()
        {
            // Arrange: Kullanıcı A oturum açmış, fakat kötü niyetli istek gövdesinde Kullanıcı B'nin ID'sini gönderiyor
            var loggedInUserId = Guid.NewGuid();
            var attackerTargetUserId = Guid.NewGuid();
            SetUserContext(loggedInUserId);

            var command = new CreateOrderingCommand
            {
                UserId = attackerTargetUserId,
                TotalPrice = 500,
                OrderDate = DateTime.UtcNow
            };

            CreateOrderingCommand capturedCommand = null;
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateOrderingCommand>(), It.IsAny<CancellationToken>()))
                         .Callback<IRequest<Guid>, CancellationToken>((req, ct) => capturedCommand = req as CreateOrderingCommand)
                         .ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _controller.CreateOrdering(command);

            // Assert: İstek gövdesindeki ID ezilmeli ve oturumdaki kullanıcının ID'si atanmalıdır
            result.Should().BeOfType<OkObjectResult>();
            capturedCommand.Should().NotBeNull();
            capturedCommand.UserId.Should().Be(loggedInUserId);
            capturedCommand.UserId.Should().NotBe(attackerTargetUserId);
        }

        [Fact]
        public async Task CreateOrdering_Should_Return_Unauthorized_When_User_Not_Authenticated()
        {
            // Arrange: Anonim istek
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var command = new CreateOrderingCommand { TotalPrice = 100 };

            // Act
            var result = await _controller.CreateOrdering(command);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task GetOrderingById_Should_Return_Forbid_When_User_Accesses_Another_Users_Order_IDOR_Protection()
        {
            // Arrange: Oturum açan kullanıcı A, sorgulanan sipariş kullanıcı B'ye ait
            var loggedInUserId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            SetUserContext(loggedInUserId, role: "Customer");

            var orderId = Guid.NewGuid();
            var orderResult = new GetOrderingByIdQueryResult
            {
                Id = orderId,
                UserId = ownerUserId,
                TotalPrice = 250,
                OrderDate = DateTime.UtcNow
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetOrderingByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(orderResult);

            // Act
            var result = await _controller.GetOrderingById(orderId);

            // Assert: IDOR koruması devreye girmeli ve 403 Forbid dönmeli
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task GetOrderingById_Should_Return_Ok_When_User_Accesses_Own_Order()
        {
            // Arrange: Oturum açan kullanıcı kendi siparişini sorguluyor
            var loggedInUserId = Guid.NewGuid();
            SetUserContext(loggedInUserId, role: "Customer");

            var orderId = Guid.NewGuid();
            var orderResult = new GetOrderingByIdQueryResult
            {
                Id = orderId,
                UserId = loggedInUserId,
                TotalPrice = 250,
                OrderDate = DateTime.UtcNow
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetOrderingByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(orderResult);

            // Act
            var result = await _controller.GetOrderingById(orderId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(orderResult);
        }

        [Fact]
        public async Task GetOrderingById_Should_Return_Ok_When_Admin_Accesses_Any_Order()
        {
            // Arrange: Admin rolündeki kullanıcı başkasının siparişini sorguluyor
            var adminId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            SetUserContext(adminId, role: "Admin");

            var orderId = Guid.NewGuid();
            var orderResult = new GetOrderingByIdQueryResult
            {
                Id = orderId,
                UserId = ownerUserId,
                TotalPrice = 400,
                OrderDate = DateTime.UtcNow
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetOrderingByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(orderResult);

            // Act
            var result = await _controller.GetOrderingById(orderId);

            // Assert: Admin için izin verilmeli
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(orderResult);
        }

        [Fact]
        public async Task RemoveOrdering_Should_Return_Forbid_When_User_Attempts_To_Delete_Another_Users_Order()
        {
            // Arrange: Kullanıcı A, kullanıcı B'nin siparişini silmeye çalışıyor
            var loggedInUserId = Guid.NewGuid();
            var ownerUserId = Guid.NewGuid();
            SetUserContext(loggedInUserId, role: "Customer");

            var orderId = Guid.NewGuid();
            var existingOrder = new GetOrderingByIdQueryResult
            {
                Id = orderId,
                UserId = ownerUserId,
                TotalPrice = 300
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetOrderingByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(existingOrder);

            // Act
            var result = await _controller.RemoveOrdering(orderId);

            // Assert: 403 Forbid dönmeli
            result.Should().BeOfType<ForbidResult>();
            _mediatorMock.Verify(m => m.Send(It.IsAny<RemoveOrderingCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task OrderingList_Should_Filter_By_Current_User_When_Caller_Is_Not_Admin()
        {
            // Arrange: Listede farklı kullanıcılara ait siparişler var
            var loggedInUserId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            SetUserContext(loggedInUserId, role: "Customer");

            var orders = new List<GetOrderingQueryResult>
            {
                new GetOrderingQueryResult { Id = Guid.NewGuid(), UserId = loggedInUserId, TotalPrice = 100 },
                new GetOrderingQueryResult { Id = Guid.NewGuid(), UserId = otherUserId, TotalPrice = 200 },
                new GetOrderingQueryResult { Id = Guid.NewGuid(), UserId = loggedInUserId, TotalPrice = 300 }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetOrderingQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(orders);

            // Act
            var result = await _controller.OrderingList();

            // Assert: Sadece oturum açan kullanıcının 2 siparişi listelenmeli
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var filteredList = okResult.Value.Should().BeAssignableTo<IEnumerable<GetOrderingQueryResult>>().Subject;
            filteredList.Should().HaveCount(2);
            filteredList.Should().OnlyContain(x => x.UserId == loggedInUserId);
        }
    }
}
