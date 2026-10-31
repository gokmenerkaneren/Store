using Microsoft.EntityFrameworkCore;
using Store.Location.Application.Interfaces;
using Store.Location.Persistence.Context;
using Store.Location.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly LocationContext _context;
        private readonly IUserContext _userContext;

        public Repository(LocationContext context, IUserContext userContext)
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

        public IQueryable<T> GetQueryable(bool includeDeleted = false)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }
            return query;
        }

        public async Task<List<T>> GetAllAsync(bool includeDeleted = false)
        {
            IQueryable<T> query = GetQueryable(includeDeleted);

            // EĞER SİLİNENLERİ DE İSTİYORSAK, EF CORE'UN GLOBAL FİLTRESİNİ DEVRE DIŞI BIRAKIYORUZ!
            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetByFilterAsync(Expression<Func<T, bool>> filter)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(filter);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
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

        public async Task RestoreAsync(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = false;
                baseEntity.IsActive = true;
                baseEntity.UpdateUserId = _userContext.GetClientId();
                baseEntity.UpdateDate = BaseEntity.GetLocalTime();
                // DeleteDate ve DeleteUserId'ye dokunulmaz, tarihsel kayıt korunur.
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new NotSupportedException($"'{typeof(T).Name}' tipi restore işlemini desteklemiyor.");
            }
        }

        public async Task<List<T>> GetAllByFilterAsync(Expression<Func<T, bool>> filter, bool includeDeleted = false)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query.Where(filter).ToListAsync();
        }

    }
}


