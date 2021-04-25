using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using System;
using System.Collections;
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
        Task<bool> RecoverPasswordForUser(string userId, string newPassword);
        Task<UserModel> GetByID(string id);
        Task<bool> RequestVerify(RequestEmailModel model);
        Task<bool> RequestNewPassword(string email);
        Task<bool> ApprovePhoto(int photoId);
        Task<bool> DeniedPhoto(DeniedPhotoModel model);
        Task<bool> Activate(string id);
        Task<bool> Follow(FollowModel model);
        bool CheckFollow(FollowModel model);
        Task<bool> Unfollow(FollowModel model);
        IEnumerable<PhotoModel> GetAllPhotoApproved(string userId);
        IEnumerable<PhotoModel> GetAllPendingPhoto(string userId);
        IEnumerable<PhotoModel> GetAllDeniedPhoto(string userId);
        IEnumerable<UserFollowProfileModel> GetAllFollowingUser(String userId);
        Task<UserFollowProfileModel> GetProfileByID(string id);
        PhotoStatusStatisticModel GetPhotoStatusStatisticByUserID(string userId);
        IEnumerable<Dictionary<string, double>> GetSellStatisticByUserIDAndTime(StatisicModel model);
        IEnumerable<PhotoModelGetAll> GetAllPhotoOfAllUserWeAreFollowing(string userId);
        IEnumerable<UserNotFollowModel> GetAllUserWeNotFollowing(string userId);
        IEnumerable<PhotoModel> GetAllNormalPhotoApproved(string userId);
        IEnumerable<PhotoModel> GetAllExclusivePhotoApproved(string userId);
        IEnumerable<PhotoModel> GetAllExclusivePropertyPhotoApproved(string userId);
        IEnumerable<Dictionary<string, string>> CheckNotification(string userId);
        Task<bool> DeleteNotification(string userId, string followUserId);
    }
}
