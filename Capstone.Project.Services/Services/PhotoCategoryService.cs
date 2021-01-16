using AutoMapper;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.Services
{
    public class PhotoCategoryService : IPhotoCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PhotoCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<PhotoModelGetAll> GetPhotoByCategory(int id)
        {
            var list = _unitOfWork.PhotoCategoryRepository.GetByObject(c => c.CategoryId == id);
            if(list != null)
            {
                List<PhotoModelGetAll> result = new List<PhotoModelGetAll>();
                foreach (var item in list)
                {
                    var photo = _unitOfWork.PhotoRepository.GetById(item.PhotoId).Result;
                    result.Add(_mapper.Map<PhotoModelGetAll>(photo));

                }
                return result;
            }
            return null;
        }
        public List<CategoryModel> GetCategoryByPhoto(int id)
        {
            var list = _unitOfWork.PhotoCategoryRepository.GetByObject(c => c.PhotoId == id);
            if (list != null)
            {
                List<CategoryModel> result = new List<CategoryModel>();
                foreach (var item in list)
                {
                    var category = _unitOfWork.CategoryRepository.GetById(item.CategoryId).Result;
                    result.Add(_mapper.Map<CategoryModel>(category));

                }
                return result;
            }
            return null;
        }
        public List<CategoryModel> GetCategoryByPhoto(int id)
        {
            var list = _unitOfWork.PhotoCategoryRepository.GetByObject(c => c.PhotoId == id);
            if (list != null)
            {
                List<CategoryModel> result = new List<CategoryModel>();
                foreach (var item in list)
                {
                    var category = _unitOfWork.CategoryRepository.GetById(item.CategoryId).Result;
                    result.Add(_mapper.Map<CategoryModel>(category));

                }
                return result;
            }
            return null;
        }
    }
}
