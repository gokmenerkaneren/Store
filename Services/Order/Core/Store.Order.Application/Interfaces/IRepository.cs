using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // includeDeleted parametresi ekliyoruz, varsayılanı false yapıyoruz ki eski kodlar bozulmasın.
        Task<List<T>> GetAllAsync(bool includeDeleted = false);
        Task<T> GetByIdAsync(Guid id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SoftDeleteAsync(T entity);
        Task<T> GetByFilterAsync(Expression<Func<T, bool>> filter);
    }
}

