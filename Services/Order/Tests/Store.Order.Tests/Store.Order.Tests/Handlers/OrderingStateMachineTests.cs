using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Store.Order.Application.Features.Mediator.Commands.OrderingCommands;
using Store.Order.Application.Features.Mediator.Handlers.OrderingHandlers;
using Store.Order.Application.Features.Mediator.Queries.OrderingQueries;
using Store.Order.Application.Features.Mediator.Results.OrderingResults;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using Store.Order.Domain.Enums;
using Store.Order.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Store.Order.Tests.Handlers
{
    public class OrderingStateMachineTests
    {
        private static readonly (OrderStatus From, OrderStatus To)[] ValidTransitions =
        {
            (OrderStatus.Pending, OrderStatus.Processing),
            (OrderStatus.Pending, OrderStatus.Cancelled),
            (OrderStatus.Processing, OrderStatus.Shipped),
            (OrderStatus.Processing, OrderStatus.Cancelled),
            (OrderStatus.Shipped, OrderStatus.Delivered),
            (OrderStatus.Shipped, OrderStatus.Returned),
            (OrderStatus.Delivered, OrderStatus.Completed),
            (OrderStatus.Delivered, OrderStatus.Returned)
        };

        public static IEnumerable<object[]> ValidTransitionData =>
            ValidTransitions.Select(t => new object[] { t.From, t.To });

        // Geçerli olarak tanımlanmayan tüm (kaynak, hedef) çiftleri; aynı duruma geçiş dahil
        public static IEnumerable<object[]> InvalidTransitionData =>
            from source in Enum.GetValues<OrderStatus>()
            from target in Enum.GetValues<OrderStatus>()
            where !ValidTransitions.Contains((source, target))
            select new object[] { source, target };

        [Theory]
        [MemberData(nameof(ValidTransitionData))]
        public void ChangeStatus_Should_Update_Status_When_Transition_Is_Valid(OrderStatus from, OrderStatus to)
        {
            var ordering = new Ordering { Status = from };

            ordering.CanTransitionTo(to).Should().BeTrue();
            ordering.ChangeStatus(to);

            ordering.Status.Should().Be(to);
        }

        [Theory]
        [MemberData(nameof(InvalidTransitionData))]
        public void ChangeStatus_Should_Throw_And_Keep_Status_When_Transition_Is_Invalid(OrderStatus from, OrderStatus to)
        {
            var ordering = new Ordering { Status = from };

            ordering.CanTransitionTo(to).Should().BeFalse();
            var act = () => ordering.ChangeStatus(to);

            act.Should().Throw<InvalidOperationException>();
            ordering.Status.Should().Be(from);
        }

        [Fact]
        public async Task Handler_Should_Apply_Valid_Status_Change()
        {
            var ordering = new Ordering { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Status = OrderStatus.Pending };
            var (handler, repository) = CreateHandler(ordering);

            await handler.Handle(new UpdateOrderingCommand
            {
                Id = ordering.Id,
                UserId = ordering.UserId,
                TotalPrice = 100,
                Status = OrderStatus.Processing
            }, CancellationToken.None);

            ordering.Status.Should().Be(OrderStatus.Processing);
            repository.Verify(r => r.UpdateAsync(ordering), Times.Once);
        }

        [Fact]
        public async Task Handler_Should_Reject_Invalid_Status_Change_Without_Saving()
        {
            var ordering = new Ordering { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Status = OrderStatus.Cancelled };
            var (handler, repository) = CreateHandler(ordering);

            var act = () => handler.Handle(new UpdateOrderingCommand
            {
                Id = ordering.Id,
                UserId = ordering.UserId,
                Status = OrderStatus.Processing
            }, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
            ordering.Status.Should().Be(OrderStatus.Cancelled);
            repository.Verify(r => r.UpdateAsync(It.IsAny<Ordering>()), Times.Never);
        }

        [Fact]
        public async Task Handler_Should_Not_Throw_When_Status_Is_Unchanged()
        {
            var ordering = new Ordering { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Status = OrderStatus.Delivered };
            var (handler, repository) = CreateHandler(ordering);

            await handler.Handle(new UpdateOrderingCommand
            {
                Id = ordering.Id,
                UserId = ordering.UserId,
                TotalPrice = 75,
                Status = OrderStatus.Delivered
            }, CancellationToken.None);

            ordering.TotalPrice.Should().Be(75);
            repository.Verify(r => r.UpdateAsync(ordering), Times.Once);
        }

        [Fact]
        public async Task Handler_Should_Reject_Transferring_Order_To_Another_User()
        {
            var ownerId = Guid.NewGuid();
            var ordering = new Ordering { Id = Guid.NewGuid(), UserId = ownerId, Status = OrderStatus.Pending };
            var (handler, repository) = CreateHandler(ordering);

            var act = () => handler.Handle(new UpdateOrderingCommand
            {
                Id = ordering.Id,
                UserId = Guid.NewGuid(),
                Status = OrderStatus.Pending
            }, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
            ordering.UserId.Should().Be(ownerId);
            repository.Verify(r => r.UpdateAsync(It.IsAny<Ordering>()), Times.Never);
        }

        [Fact]
        public async Task Controller_Should_Preserve_Protected_Fields_When_Caller_Is_Order_Owner()
        {
            var ownerId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var newDate = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.Is<GetOrderingByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetOrderingByIdQueryResult { Id = orderId, UserId = ownerId, TotalPrice = 500, Status = OrderStatus.Pending });

            var command = new UpdateOrderingCommand
            {
                Id = orderId,
                UserId = Guid.NewGuid(),
                TotalPrice = 1,
                OrderDate = newDate,
                Status = OrderStatus.Delivered
            };

            var result = await CreateController(mediator, ownerId, "Customer").UpdateOrdering(command);

            result.Should().BeOfType<OkObjectResult>();
            mediator.Verify(m => m.Send(It.Is<UpdateOrderingCommand>(c =>
                c.UserId == ownerId &&
                c.TotalPrice == 500 &&
                c.Status == OrderStatus.Pending &&
                c.OrderDate == newDate), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Controller_Should_Forbid_When_Caller_Does_Not_Own_Order()
        {
            var orderId = Guid.NewGuid();
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.Is<GetOrderingByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetOrderingByIdQueryResult { Id = orderId, UserId = Guid.NewGuid() });

            var result = await CreateController(mediator, Guid.NewGuid(), "Customer")
                .UpdateOrdering(new UpdateOrderingCommand { Id = orderId });

            result.Should().BeOfType<ForbidResult>();
            mediator.Verify(m => m.Send(It.IsAny<UpdateOrderingCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Controller_Should_Return_BadRequest_When_Admin_Requests_Invalid_Transition()
        {
            var orderId = Guid.NewGuid();
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.Is<GetOrderingByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetOrderingByIdQueryResult { Id = orderId, UserId = Guid.NewGuid(), Status = OrderStatus.Cancelled });
            mediator.Setup(m => m.Send(It.IsAny<UpdateOrderingCommand>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new InvalidOperationException("Geçersiz geçiş"));

            var result = await CreateController(mediator, Guid.NewGuid(), "Admin")
                .UpdateOrdering(new UpdateOrderingCommand { Id = orderId, Status = OrderStatus.Processing });

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private static (UpdateOrderingCommandHandler Handler, Mock<IRepository<Ordering>> Repository) CreateHandler(Ordering ordering)
        {
            var repository = new Mock<IRepository<Ordering>>();
            repository.Setup(r => r.GetByIdAsync(ordering.Id)).ReturnsAsync(ordering);
            repository.Setup(r => r.UpdateAsync(It.IsAny<Ordering>())).Returns(Task.CompletedTask);
            return (new UpdateOrderingCommandHandler(repository.Object), repository);
        }

        private static OrderingsController CreateController(Mock<IMediator> mediator, Guid userId, string role)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            }, "TestAuthType");

            return new OrderingsController(mediator.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
                }
            };
        }
    }
}
