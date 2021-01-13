using AutoMapper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.Repository;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.Services
{
    public class CategoryService : BaseService<Category, CategoryModel>, ICategoryService
    {
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
        protected override IGenericRepository<Category> _reponsitory => _unitOfWork.CategoryRepository;

        public override async Task<CategoryModel> CreateAsync(CategoryModel dto)
        {
            if (await _unitOfWork.CategoryRepository.GetFirst(c => c.CategoryName == dto.CategoryName)!= null)
            {
                return null;
            }
            var entity = _mapper.Map<Category>(dto);
            entity.DelFlg = false;
            _reponsitory.Add(entity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<CategoryModel>(entity);
        }
    }
}
