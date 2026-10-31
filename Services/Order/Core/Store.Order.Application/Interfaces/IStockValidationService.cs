using System;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Order.Application.Interfaces
{
    public interface IStockValidationService
    {
        Task<bool> ValidateStockAsync(Guid productId, int requestedQuantity, CancellationToken cancellationToken = default);
    }
}
