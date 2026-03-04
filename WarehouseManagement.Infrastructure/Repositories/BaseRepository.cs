using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WarehouseManagement.Domain.Common;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class BaseRepository<T>(DbContext context) : IRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public virtual async Task<IEnumerable<T>> GetAllAsync(
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
