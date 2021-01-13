using AutoMapper;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;

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


    }
}
