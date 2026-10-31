using Store.B2B.Domain.Entities;
using Store.Shared.Entities;

namespace Store.B2B.Application.Interfaces
{
    /// <summary>
    /// B2B servisi için generic repository arayüzü.
    /// </summary>
    public interface IB2BRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}
