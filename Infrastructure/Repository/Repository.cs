using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly RoomContainerContext _hotelisContext;
        private DbSet<T> dbSet;
        public Repository(RoomContainerContext hotelisContext)
        {
            _hotelisContext = hotelisContext;

            dbSet = _hotelisContext.Set<T>();
        }
        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> where, Func<IQueryable<T>, IIncludableQueryable<T, object>> include)
        {
            IQueryable<T> query = dbSet;

            if (include != null)
                query = include(query);

            return await query.Where(where).FirstAsync();
        }
        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> where)
        {
            IQueryable<T> query = dbSet;

            return await query.Where(where).FirstAsync();
        }

        public async Task<IEnumerable<T>> GetAllByIdAsync(Expression<Func<T, bool>> where, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, Func<IQueryable<T>, IIncludableQueryable<T, object>> include)
        {
            IQueryable<T> query = dbSet;

            if (where != null)
            {
                query = query.Where(where);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                return await orderBy(query).AsNoTracking().ToListAsync();
            }
            else
            {
                return await query.AsNoTracking().ToListAsync();
            }
        }
        public async Task<IEnumerable<T>> GetAllByIdAsync(Expression<Func<T, bool>> where, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            IQueryable<T> query = dbSet;

            if (where != null)
                query = query.Where(where);

            if (orderBy != null)
                return await orderBy(query).AsNoTracking().ToListAsync();
            else
                return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllByIdAsync(Expression<Func<T, bool>> where, Func<IQueryable<T>, IIncludableQueryable<T, object>> include)
        {
            IQueryable<T> query = dbSet;

            if (where != null)
                query = query.Where(where);

            if (include != null)
                query = include(query);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllByIdAsync(Expression<Func<T, bool>> where)
        {
            IQueryable<T> query = dbSet;

            if (where != null)
                query = query.Where(where);

            return await query.AsNoTracking().ToListAsync();
        }

        public Task UpdateAsync(T entity)
        {
            dbSet.Attach(entity);
            _hotelisContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }


        public async Task DeleteAsync(int id)
        {
            if (dbSet != null)
            {
                if (id > 0)
                {
                    var entityToDelete = await dbSet.FindAsync(id);
                    if (entityToDelete is not null)
                    {
                        await DeleteAsync(entityToDelete);
                    }
                }
            }
        }
        private Task DeleteAsync(T entity)
        {
            if (_hotelisContext.Entry(entity).State == EntityState.Deleted)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<EntityEntry<T>> CreateAsync(T entity)
        {
            var result = await dbSet.AddAsync(entity);
            await SaveChagesAsync();
            return result;
        }
        private async Task SaveChagesAsync()
        {
            await _hotelisContext.SaveChangesAsync();
        }

        public async Task<bool> Exist(Expression<Func<T, bool>> where)
        {
            return await dbSet.AsNoTracking().AnyAsync(where);
        }


    }
}