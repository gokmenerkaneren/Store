using Microsoft.EntityFrameworkCore;
using Store.B2B.Application.Interfaces;
using Store.B2B.Domain.Entities;
using Store.B2B.Persistence.Context;
using Store.Shared.Entities;

namespace Store.B2B.Persistence.Repositories
{
    /// <summary>
    /// B2B servisi için EF Core generic repository implementasyonu.
    /// </summary>
    public class B2BRepository<T> : IB2BRepository<T> where T : BaseEntity
    {
        private readonly B2BContext _context;

        public B2BRepository(B2BContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().Where(x => !x.IsDeleted).AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task CreateAsync(T entity)
        {
            entity.InsertDate = BaseEntity.GetLocalTime();
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            entity.UpdateDate = BaseEntity.GetLocalTime();
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity is not null && !entity.IsDeleted)
            {
                entity.IsDeleted = true;
                entity.IsActive = false;
                entity.DeleteDate = BaseEntity.GetLocalTime();
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
