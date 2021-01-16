using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Capstone.Project.Data.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal CapstoneProjectContext _context;
        internal DbSet<TEntity> _dbSet;

        public GenericRepository(CapstoneProjectContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<PaginatedList<TEntity>> Get(int pageIndex = 0, int pageSize = 0, Expression<Func<TEntity, bool>> filter = null,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await PaginatedList<TEntity>.CreateAsync(query.AsNoTracking(), pageIndex, pageSize);
        }


        public virtual async Task<TEntity> GetById(object id)
        {
            return await _dbSet.FindAsync(id);
        }


        public virtual void Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentException("entity");
            _dbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentException("entity");
            _dbSet.Attach(entity);
            _dbSet.Update(entity);
        }


        public virtual void Delete(object id)
        {
            TEntity entity = _dbSet.Find(id);
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public virtual IQueryable<TEntity> GetByObject(Expression<Func<TEntity, bool>> filter, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;
            query = query.Where(filter);
            foreach (var includeProperty in includeProperties.Split
                                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public virtual IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query;
        }

        public Task<TEntity> GetFirst(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefaultAsync();
        }
    }
}
