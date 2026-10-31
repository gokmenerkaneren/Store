using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Order.Application.Interfaces;
using Store.Order.Persistence.Context;
using Store.Order.Domain.Entities;
using Store.Shared.Entities;

namespace Store.Order.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly OrderContext _context;
        private readonly IUserContext _userContext;

        public Repository(OrderContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task CreateAsync(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.InsertUserId = _userContext.GetClientId();
                baseEntity.InsertDate = BaseEntity.GetLocalTime();
            }
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            await SoftDeleteAsync(entity);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByFilterAsync(Expression<Func<T, bool>> filter)
        {
            var values = await _context.Set<T>().SingleOrDefaultAsync(filter);
            if (values == null)
            {
                throw new KeyNotFoundException($"'{typeof(T).Name}' tipinde, belirtilen kriterlere uygun hiçbir kayıt bulunamadı.");
            }
            return values;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"'{typeof(T).Name}' tipinde ve {id} numaral ID'ye sahip kayt bulunamad.");
            }

            return entity;

        }

        public async Task UpdateAsync(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.UpdateUserId = _userContext.GetClientId();
                baseEntity.UpdateDate = BaseEntity.GetLocalTime();
            }
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;
                baseEntity.IsActive = false;
                baseEntity.DeleteUserId = _userContext.GetClientId();
                baseEntity.DeleteDate = BaseEntity.GetLocalTime();
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new NotSupportedException($"'{typeof(T).Name}' tipi soft delete işlemini desteklemiyor.");
            }
        }

        public async Task<List<T>> GetAllAsync(bool includeDeleted = false)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }
            return await query.ToListAsync();
        }
    }
}


