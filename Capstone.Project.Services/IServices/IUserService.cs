using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.IServices
{
    public interface IUserService
    {
        Task<UserModel> LoginGoogle(string uid, string username, string password);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetByUserName(string username, string action = "");
        Task<User> CreateUser(RegisterModel model, string password);
        Task<bool> CheckPassWord(string username, string password);
         UserModel UpdateUser(string id, UserUpdateModel userUpdateModel);
        Task<bool> CheckPasswordToUpdate(string username, string oldPassword, string newPassword);
        Task<UserModel> GetByID(string id);
        Task<bool> RequestVerify(RequestEmailModel model);

        Task<bool> Activate(string id);
    }
}
