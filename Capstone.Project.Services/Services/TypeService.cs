using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Services.Services
{
    public class TypeService : ITypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        IEnumerable<Data.Models.Type> ITypeService.GetAllType()
        {
            return _unitOfWork.TypeRepository.GetAll();
        }
    }
}
