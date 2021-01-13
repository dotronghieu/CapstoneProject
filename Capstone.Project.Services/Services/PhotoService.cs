using AutoMapper;
using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.Repository;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Services.Services
{
    public class PhotoService : BaseService<Photo, PhotoModel>, IPhotoService
    {
        public PhotoService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
        protected override IGenericRepository<Photo> _reponsitory => _unitOfWork.PhotoRepository;
        //public override async Task<PhotoModel> CreateAsync(PhotoModel dto)
        //{
        //    var entity = _mapper.Map<Photo>(dto);
        //    entity.DelFlg = false;
        //    entity.InsDateTime = DateTime.Now;
            

        //}

    }
}
    