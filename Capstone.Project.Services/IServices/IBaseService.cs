using Capstone.Project.Data.Helper;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Capstone.Project.Services
{
    public interface IBaseService<TEntity, TDto> 
        where TEntity : class 
        where TDto : class
    {
        Task<PaginatedList<TEntity>> GetAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> filter = null,
                                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                        string includeProperties = "");
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
                          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                          string includeProperties = "");
        Task<TDto> CreateAsync(TDto dto);
        Task<TDto> UpdateAsync(TDto dto);
        Task<bool> DeleteAsync(object id);
        Task<TDto> GetByIdAsync(object id);
    }
}
