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

        public List<PhotoModel> GetPhotoByCategory(int id)
        {
            var list = _unitOfWork.PhotoCategoryRepository.GetByObject(c => c.CategoryId == id);
            if(list != null)
            {
                List<PhotoModel> result = new List<PhotoModel>();
                foreach (var item in list)
                {
                    var photo = _unitOfWork.PhotoRepository.GetById(item.PhotoId).Result;
                    result.Add(_mapper.Map<PhotoModel>(photo));

                }
                return result;
            }
            return null;
        }
    }
}
