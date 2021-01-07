using AutoMapper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<UserModel, User>();
            CreateMap<RegisterModel, User>();

            CreateMap<Role, RoleModel>();
            CreateMap<RoleModel, Role>();

        }
    }
}
