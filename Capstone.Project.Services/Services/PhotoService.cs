﻿using AutoMapper;
using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.Repository;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.Services
{
    public class PhotoService : BaseService<Photo, PhotoModel>, IPhotoService
    {
        private readonly CapstoneProjectContext _context;
        public PhotoService(IUnitOfWork unitOfWork, IMapper mapper, CapstoneProjectContext capstoneProjectContext) : base(unitOfWork, mapper)
        {
            _context = capstoneProjectContext;
        }
        protected override IGenericRepository<Photo> _reponsitory => _unitOfWork.PhotoRepository;

        public IEnumerable<PhotoModelGetAll> GetRandomPhoto()
        {
            List<PhotoModelGetAll> resultList = new List<PhotoModelGetAll>();
            var sourceList =   _reponsitory.GetAll(filter: c => c.DelFlg == false).ToList();    
        
            if(sourceList != null)
            {
                Random rnd = new Random();
                int skip = rnd.Next(1, 3);
                var list = sourceList.Skip(skip).Take(20);
                foreach (var item in list)
                {
                    resultList.Add(_mapper.Map<PhotoModelGetAll>(item));
                }
                return resultList.AsEnumerable<PhotoModelGetAll>();
            }
            return null;
        }

        public PhotoModel UpdatePhoto(int id, PhotoModel model)
        {
            var entity =  _reponsitory.GetById(id).Result;
            if(entity != null)
            {
                entity.PhotoName = model.PhotoName;
                entity.Price = model.Price;
                entity.TypeId = model.TypeId;
                _reponsitory.Update(entity);
                _context.SaveChanges();
                return _mapper.Map<PhotoModel>(entity);
            }
       
            return null;
        }
   

        //public override async Task<PhotoModel> CreateAsync(PhotoModel dto)
        //{
        //    var entity = _mapper.Map<Photo>(dto);
        //    entity.DelFlg = false;
        //    entity.InsDateTime = DateTime.Now;


        //}

    }
}
    