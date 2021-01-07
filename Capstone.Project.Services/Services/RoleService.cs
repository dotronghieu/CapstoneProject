using AutoMapper;
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
    public class RoleService : BaseService<Role, RoleModel>, IRoleService
    {
        public RoleService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        protected override IGenericRepository<Role> _reponsitory => _unitOfWork.RoleRepository;

        public string GetRole(User user)
        {
            var role = _reponsitory.GetByObject(u => u.RoleId == user.RoleId).SingleOrDefault();
            return role.RoleName;
        }
        public override async Task<RoleModel> CreateAsync(RoleModel dto)
        {
            var entity = _mapper.Map<Role>(dto);

            _reponsitory.Add(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<RoleModel>(entity);
        }
    }
}
