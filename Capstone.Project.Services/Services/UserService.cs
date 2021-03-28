using AutoMapper;
using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using FirebaseAdmin.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.Services
{
    public class UserService :  IUserService  
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
 
        public async Task<bool> CheckPassWord(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))   
            {
                return false;
            }

            var user = await _unitOfWork.UsersRepository.GetByUsername(username);

            if (user != null)
            {
                if (VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<User> CreateUser(RegisterModel model, string password)
        {
            if (await _unitOfWork.UsersRepository.GetByUsername(model.Username) == null)
            {
                var user = _mapper.Map<User>(model);
                user.RoleId = Constants.Const.ROLE_USER_ID;
                user.UserId = Guid.NewGuid().ToString();
                user.EncryptCode = Guid.NewGuid().ToString();
                user.IsVerify = false;
                await _unitOfWork.UsersRepository.Create(user, password);
                await _unitOfWork.SaveAsync();
                return user;
            }
            return null;
            
        }
            
        public async Task<User> GetByUserName(string username, string action)
        {
            var entity = await _unitOfWork.UsersRepository.GetByUsername(username);
            if (entity == null)
            {
                return null;
            }
            return entity;
        }


        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var User = await _unitOfWork.UsersRepository.GetAll();
            return User;
        }

        public  UserModel UpdateUser(string id, UserUpdateModel userUpdateModel)
        {
                var entity = _mapper.Map<User>(userUpdateModel);
                entity.UserId = id;
                bool check = _unitOfWork.UsersRepository.Update(entity);
                if(check)
            {
                return _mapper.Map<UserModel>(entity);
            } return null;
                
        }

        public async Task<bool> CheckPasswordToUpdate(string username, string oldPassword, string newPassword)
        {
            if ( await this.CheckPassWord(username, oldPassword))
            {
                var user = await _unitOfWork.UsersRepository.GetByUsername(username);
                byte[] passwordHash, passwordSalt;
                _unitOfWork.UsersRepository.CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _unitOfWork.UserGenRepository.Update(user);
                await _unitOfWork.SaveAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> RecoverPasswordForUser(string userId, string newPassword)
        {          
                var user = await _unitOfWork.UsersRepository.GetById(userId);
                byte[] passwordHash, passwordSalt;
                _unitOfWork.UsersRepository.CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _unitOfWork.UserGenRepository.Update(user);
                await _unitOfWork.SaveAsync();
                return true;
        }
        public async Task<UserModel> LoginGoogle(string uid, string username, string password)
        {
            UserRecord user_firebase = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            var currentUser = await _unitOfWork.UsersRepository.GetById(uid);

            if (currentUser == null)
            {
                var user_info = new User()
                {
                    UserId = uid,
                    Username = username,
                    RoleId = Constants.Const.ROLE_USER_ID,
                    Email = user_firebase.Email,
                    FullName = user_firebase.DisplayName,
                    IsVerify = true,
                    DelFlg = false,
                };

                await _unitOfWork.UsersRepository.Create(user_info, password);

                if (await _unitOfWork.SaveAsync() > 0)
                {
                    return _mapper.Map<UserModel>(user_info);
                }
                else
                {
                    throw new Exception("Create new account failed");
                }
            }
            return _mapper.Map<UserModel>(currentUser);
        }

        public async Task<bool> RequestVerify(RequestEmailModel model)
        {
            var user = await _unitOfWork.UsersRepository.GetById(model.UserId);
            if(model.Email == user.Email)
            {
                var verifyUrl = "https://imago.azurewebsites.net//api/v1/Auth/Verify/" + model.UserId;
                var fromMail = new MailAddress(Constants.Const.IMAGO_EMAIL, "Imago (No Reply)");
                var toMail = new MailAddress(model.Email);
                var imagoPassword = Constants.Const.IMAGO_EMAIL_PASSWORD;
                string subject = "Your account is successfull created";
                string body = "<br/><br/>We are excited to tell you that your account is" +
                  " successfully created. Please click on the below link to verify your account" +
                  " <br/><br/><a href='" + verifyUrl + "'>" + "Click here to verify" + "</a> ";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromMail.Address, imagoPassword)

                };
                using (var message = new MailMessage(fromMail, toMail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);
                return true;
            }
            return false;
           
        }

        public async Task<bool> Activate(string id)
        {
            var user = await _unitOfWork.UserGenRepository.GetById(id);
            if(user != null)
            {
                user.IsVerify = true;
               await  _unitOfWork.SaveAsync();
                return true;
            }
            return false;
        }

        public async Task<UserModel> GetByID(string id)
        {
            var user = await _unitOfWork.UserGenRepository.GetById(id);
            if(user != null)
            {
                return _mapper.Map<UserModel>(user);
            }
            return null;
        }

        public async Task<bool> ApprovePhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetById(photoId);
            if(photo != null)
            {
                photo.ApproveStatus = Constants.Const.PHOTO_STATUS_APPROVED;
                _unitOfWork.PhotoRepository.Update(photo);
                await _unitOfWork.SaveAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> DeniedPhoto(DeniedPhotoModel model)
        {
            var photo = await _unitOfWork.PhotoRepository.GetById(model.Id);
            if (photo != null)
            {
                photo.ApproveStatus = Constants.Const.PHOTO_STATUS_DENIED;
                photo.Note = model.Reason;
                photo.Description = model.Description;
                _unitOfWork.PhotoRepository.Update(photo);
                await _unitOfWork.SaveAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<PhotoModel> GetAllPhotoApproved(string userId)
        {
            var listPhotoApproved =  _unitOfWork.PhotoRepository.GetByObject(p => p.DelFlg == false && p.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED && p.UserId == userId).ToList();
            if(listPhotoApproved != null)
            {
                List<PhotoModel> result = new List<PhotoModel>();
                foreach (var item in listPhotoApproved)
                {
                    result.Add(_mapper.Map<PhotoModel>(item));
                }
                return result.AsEnumerable<PhotoModel>();
            }
            return null;

        }

        public IEnumerable<PhotoModel> GetAllPendingPhoto(string userId)
        {
            var listPhotoPending = _unitOfWork.PhotoRepository.GetByObject(p => p.DelFlg == false && p.ApproveStatus == Constants.Const.PHOTO_STATUS_PENDING && p.UserId == userId).ToList();
            if (listPhotoPending != null)
            {
                List<PhotoModel> result = new List<PhotoModel>();
                foreach (var item in listPhotoPending)
                {
                    result.Add(_mapper.Map<PhotoModel>(item));
                }
                return result.AsEnumerable<PhotoModel>();
            }
            return null;
        }

        public IEnumerable<PhotoModel> GetAllDeniedPhoto(string userId)
        {
            var listPhotoDenied = _unitOfWork.PhotoRepository.GetByObject(p => p.DelFlg == false && p.ApproveStatus == Constants.Const.PHOTO_STATUS_DENIED && p.UserId == userId).ToList();
            if (listPhotoDenied != null)
            {
                List<PhotoModel> result = new List<PhotoModel>();
                foreach (var item in listPhotoDenied)
                {
                    result.Add(_mapper.Map<PhotoModel>(item));
                }
                return result.AsEnumerable<PhotoModel>();
            }
            return null;
        }

        public async Task<bool> RequestNewPassword(string email)
        {
            var user = await _unitOfWork.UserGenRepository.GetFirst(c => c.Email == email && c.DelFlg == false && c.IsVerify == true);
            if(user != null)
            {
                var verifyUrl = "http://localhost:8081/#/changeforgotpassword?userId=" + user.UserId;
                var fromMail = new MailAddress(Constants.Const.IMAGO_EMAIL, "Imago (No Reply)");
                var toMail = new MailAddress(email);
                var imagoPassword = Constants.Const.IMAGO_EMAIL_PASSWORD;
                string subject = "Account recovery";
                string body = "<br/><br/>Hi " + user.FullName+
                  " Your login username is : " + "<b>" + user.Username+ "</b>" +
                  "<br/>Click recovery link below to change your password" +
                  " <br/><br/><a href='" + verifyUrl + "'>" + "Recovery" + "</a> ";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromMail.Address, imagoPassword)

                };
                using (var message = new MailMessage(fromMail, toMail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);
                return true;
            }
            return false;
        }

        public async Task <bool> Follow(FollowModel model)
        {
            if(model.UserId != null && model.FollowUserId != null)
            {
                var followModel = new FollowModel()
                {
                    UserId = model.UserId,
                    FollowUserId = model.FollowUserId
                }
                ;
                _unitOfWork.FollowRepository.Add(_mapper.Map<Follow>(followModel));
                await _unitOfWork.SaveAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> Unfollow(FollowModel model)
        {
            Follow followModel = await _unitOfWork.FollowRepository.GetFirst(c => c.UserId == model.UserId && c.FollowUserId == model.FollowUserId);
            if(followModel != null)
            {
                _unitOfWork.FollowRepository.Delete2(followModel.UserId, followModel.FollowUserId);
                await _unitOfWork.SaveAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<UserFollowProfileModel> GetAllFollowingUser(string userId)
        {
            var userFollowList = _unitOfWork.FollowRepository.GetByObject(c => c.UserId == userId).ToList();
            List<UserFollowProfileModel> resultList = new List<UserFollowProfileModel>();
            if(userFollowList.Count >= 1)
            {
                foreach (var item in userFollowList)
                {
                    resultList.Add(_mapper.Map<UserFollowProfileModel>(_unitOfWork.UserGenRepository.GetById(item.FollowUserId).Result));
                }
                return resultList;
            }
            return null;
        }

        public async Task<UserFollowProfileModel> GetProfileByID(string id)
        {
            return  _mapper.Map<UserFollowProfileModel>(await _unitOfWork.UsersRepository.GetById(id));
        }

        public PhotoStatusStatisticModel GetPhotoStatusStatisticByUserID(string userId)
        {
            var numberOfApprovedPhoto = _unitOfWork.PhotoRepository.GetByObject(p => p.UserId == userId && p.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED).Count();
            var numberOfDeniedPhoto = _unitOfWork.PhotoRepository.GetByObject(p => p.UserId == userId && p.ApproveStatus == Constants.Const.PHOTO_STATUS_DENIED).Count();
            var numberOfPendingPhoto = _unitOfWork.PhotoRepository.GetByObject(p => p.UserId == userId && p.ApproveStatus == Constants.Const.PHOTO_STATUS_PENDING).Count();
            PhotoStatusStatisticModel result = new PhotoStatusStatisticModel();
            result.NumberOfApprovedPhoto = numberOfApprovedPhoto;
            result.NumberOfDeniedPhoto = numberOfDeniedPhoto;
            result.NumberOfPendingPhoto = numberOfPendingPhoto;
            return result;
        }
    }
}
