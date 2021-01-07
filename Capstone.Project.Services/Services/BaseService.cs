using AutoMapper;
using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Repository;
using Capstone.Project.Data.UnitOfWork;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Capstone.Project.Services
{
    public abstract class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        protected abstract IGenericRepository<TEntity> _reponsitory { get; }

        public BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _reponsitory.Add(entity);

            await _unitOfWork.SaveAsync();

            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            if (id != null)
            {
                _reponsitory.Delete(id);

            }
            return await _unitOfWork.SaveAsync() > 0;
        }

        public virtual async Task<TDto> UpdateAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _reponsitory.Update(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<TDto> GetByIdAsync(object id)
        {
            if (id != null)
            {
                return _mapper.Map<TDto>(await _reponsitory.GetById(id));
            }
            return null;
        }

        public Task<PaginatedList<TEntity>> GetAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return _reponsitory.Get(pageIndex, pageSize, filter, orderBy, includeProperties);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return _reponsitory.GetAll(filter, orderBy, includeProperties);
        }
    }
}
