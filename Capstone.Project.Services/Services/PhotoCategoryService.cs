using AutoMapper;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
        public PhotoCategoryService()
        {
      
        }
        public List<PhotoModel> GetPhotoByCategory(int id)
        {
            var list = _unitOfWork.PhotoCategoryRepository.GetByObject(c => c.CategoryId == id, includeProperties: "Photo").ToList();
       
            if(list != null)
            {   
                List<PhotoModel> result = new List<PhotoModel>();
                foreach (var item in list)
                {
                    //var photo = _unitOfWork.PhotoRepository.GetById(item.PhotoId).Result;
                    var photo = item.Photo;
                    result.Add(_mapper.Map<PhotoModel>(photo));

                }
                return result;
            }
            return null;
        }
        public List<CategoryModel> GetCategoryByPhoto(int id)
        {
            var list = _unitOfWork.PhotoCategoryRepository.GetByObject(c => c.PhotoId == id, includeProperties: "Category").ToList();
            if (list != null)
            {
                List<CategoryModel> result = new List<CategoryModel>();
                foreach (var item in list)
                {
                    //var category = _unitOfWork.CategoryRepository.GetById(item.CategoryId).Result;
                    var category = item.Category;
                    result.Add(_mapper.Map<CategoryModel>(category));

                }
                return result;
            }
            return null;
        }

    }
}
