using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Services.IServices
{
    public interface IRoleService : IBaseService<Role, RoleModel>
    {
        string GetRole(User user);
    }
}
