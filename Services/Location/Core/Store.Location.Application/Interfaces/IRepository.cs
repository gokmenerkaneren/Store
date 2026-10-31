using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Application.Interfaces
{
    //  Bu dışarıdan bir T değeri alacak bu bu T değeri mutlaka bir classa ait olacak.
    public interface IRepository<T> where T : class
    {
        // includeDeleted parametresi ekliyoruz, varsayılanı false yapıyoruz ki eski kodlar bozulmasın.
        IQueryable<T> GetQueryable(bool includeDeleted = false);
        Task<List<T>> GetAllAsync(bool includeDeleted = false);
        Task<T> GetByIdAsync(Guid id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SoftDeleteAsync(T entity);
        Task RestoreAsync(T entity);
        Task<T> GetByFilterAsync(Expression<Func<T, bool>> filter);

        /// Birden fazla sonuç döndüren filtreli sorgu (Örn: belirli bir Dealer'ın tüm adresleri).
        Task<List<T>> GetAllByFilterAsync(Expression<Func<T, bool>> filter, bool includeDeleted = false);
    }
}

