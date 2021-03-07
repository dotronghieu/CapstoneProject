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
            CreateMap<UserUpdateModel, User>();
            CreateMap<User, UserUpdateModel>();
            CreateMap<User, UserFollowProfileModel>();
            CreateMap<UserFollowProfileModel, User>();

            CreateMap<Role, RoleModel>();
            CreateMap<RoleModel, Role>();

            CreateMap<Photo, PhotoModel>();
            CreateMap<PhotoModel, Photo>();

            CreateMap<Photo, PhotoModelGetAll>();
            CreateMap<PhotoModelGetAll, Photo>();

            CreateMap<Photo, PhotoModelAdmin>();
            CreateMap<PhotoModelAdmin, Photo>();

            CreateMap<Category, CategoryModel>();
            CreateMap<CategoryModel, Category>();

            CreateMap<Order, OrderModel>();
            CreateMap<OrderModel, Order>();

            CreateMap<OrderDetail, OrderDetailModel>();
            CreateMap<OrderDetailModel, OrderDetail>();

            CreateMap<Follow, FollowModel>();
            CreateMap<FollowModel, Follow>();


        }
    }
}
