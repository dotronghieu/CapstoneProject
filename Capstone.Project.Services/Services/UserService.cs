﻿using AutoMapper;
using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using FirebaseAdmin.Auth;
using System;
using System.Collections;
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
                if(user.Email != null)
                {
                    var verifyUrl = "https://capstoneprojectapi20210418160622.azurewebsites.net//api/v1/Auth/Verify/" + user.UserId;
                    var fromMail = new MailAddress(Constants.Const.IMAGO_EMAIL, "Imago (No Reply)");
                    var toMail = new MailAddress(model.Email);
                    var imagoPassword = Constants.Const.IMAGO_EMAIL_PASSWORD;
                    string subject = "Your account is successfull created";
                    string body = "Hi " + user.FullName +
                        "<br/><br/>We are excited to tell you that your account is" +
                      " successfully created. Please click on the below link to verify your account" +
                      " <br/><br/><a href='" + verifyUrl + "'>" + "Click here to verify" + "</a>" +
                      " <br/><br/> Sincerely <br/>Imago";

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
                }
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
                var check = await _unitOfWork.PhotoEditRepository.GetById(photoId);
                if(check != null)
                {
                    photo.PhotoName = check.PhotoName;
                    photo.Price = check.Price;
                    photo.Description = check.Description;
                    _unitOfWork.PhotoEditRepository.Delete(check.PhotoId);
                    await _unitOfWork.SaveAsync();
                    photo.ApproveStatus = Constants.Const.PHOTO_STATUS_APPROVED;
                    _unitOfWork.PhotoRepository.Update(photo);
                    await _unitOfWork.SaveAsync();
                }
                else
                {
                    photo.ApproveStatus = Constants.Const.PHOTO_STATUS_APPROVED;
                    _unitOfWork.PhotoRepository.Update(photo);
                    await _unitOfWork.SaveAsync();

                    var listUser = _unitOfWork.FollowRepository.GetByObject(c => c.FollowUserId == photo.UserId).ToList();
                    foreach (var user in listUser)
                    {
                        Notification noti = new Notification();
                        noti.UserId = user.UserId;
                        noti.FollowUserId = user.FollowUserId;
                        noti.PhotoId = photoId;
                        noti.NotificationContent = Constants.Const.NOTIFICATION_1;
                        noti.PhotoName = photo.PhotoName;
                        noti.Wmlink = photo.Wmlink;
                        noti.IsNotified = false;
                        _unitOfWork.NotificationRepository.Add(noti);
                        await _unitOfWork.SaveAsync();
                    }
                }
                return true;
            }
            return false;
        }
        public async Task<bool> DeniedPhoto(DeniedPhotoModel model)
        {
            var photo = await _unitOfWork.PhotoRepository.GetById(model.Id);
            if (photo != null)
            {        
                photo.Note = model.Reason;
                photo.Description = model.Description;
                var editInfo = _unitOfWork.PhotoEditRepository.GetById(model.Id).Result;
                if (editInfo != null)
                {
                    _unitOfWork.PhotoEditRepository.Delete(editInfo.PhotoId);
                    photo.ApproveStatus = Constants.Const.PHOTO_STATUS_APPROVED;
                    await _unitOfWork.SaveAsync();
                }
                else 
                {
                    photo.ApproveStatus = Constants.Const.PHOTO_STATUS_DENIED;
                    _unitOfWork.PhotoRepository.Update(photo);
                    await _unitOfWork.SaveAsync();
                }         
                return true;
            }
            return false;
        }

        public IEnumerable<PhotoModel> GetAllNormalPhotoApproved(string userId)
        {
            var listPhotoApproved = _unitOfWork.PhotoRepository.GetByObject(p => p.DelFlg == false && p.TypeId == 1 && p.DisableFlg == false && p.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED && p.UserId == userId).ToList();
            if (listPhotoApproved != null)
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

        public IEnumerable<PhotoModel> GetAllExclusivePhotoApproved(string userId)
        {
            var listPhotoApproved = _unitOfWork.PhotoRepository.GetByObject(p => p.DelFlg == false && p.TypeId == 2 && p.DisableFlg == false && p.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED && p.UserId == userId).ToList();
            if (listPhotoApproved != null)
            {
                List<PhotoModel> result = new List<PhotoModel>();
                var orders = _unitOfWork.OrdersRepository.GetByObject(c => c.UserId == userId).ToList();
                foreach (var item in listPhotoApproved)
                {
                    result.Add(_mapper.Map<PhotoModel>(item));
                }
                return result.AsEnumerable<PhotoModel>();
            }
            return null;

        }

        public IEnumerable<PhotoModel> GetAllExclusivePropertyPhotoApproved(string userId)
        {
            var listPhotoApproved = _unitOfWork.PhotoRepository.GetByObject(p => p.DelFlg == false && p.TypeId == 2 && p.DisableFlg == true && p.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED && p.UserId == userId).ToList();
            if (listPhotoApproved != null)
            {
                List<PhotoModel> result = new List<PhotoModel>();
                var orders = _unitOfWork.OrdersRepository.GetByObject(c => c.UserId == userId).ToList();
                foreach (var order in orders)
                {
                    foreach (var item in listPhotoApproved)
                    {
                        var orderdetail = _unitOfWork.OrderDetailRepository.GetByObject(c => c.PhotoId == item.PhotoId && c.OrderId == order.OrderId);
                        if (orderdetail.Count() > 0)
                        {
                            result.Add(_mapper.Map<PhotoModel>(item));
                        }
                    }
                }
                return result.AsEnumerable<PhotoModel>();
            }
            return null;
        }

        public IEnumerable<PhotoModel> GetAllPhotoApproved(string userId)
        {
            var listPhotoApproved =  _unitOfWork.PhotoRepository.GetByObject(p => p.DelFlg == false && p.DisableFlg == false && p.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED && p.UserId == userId).ToList();
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
                    var editPhoto = _unitOfWork.PhotoEditRepository.GetById(item.PhotoId).Result;
                    if(editPhoto != null)
                    {
                        item.PhotoName = editPhoto.PhotoName;
                        item.Description = editPhoto.Description;
                        item.Price = editPhoto.Price;
                    }
                    result.Add(_mapper.Map<PhotoModel>(item));
                }
                return result;
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
                Follow value = new Follow();
                Follow followModel = await _unitOfWork.FollowRepository.GetFirst(c => c.UserId == model.UserId && c.FollowUserId == model.FollowUserId);
                if (followModel != null)
                {
                    followModel.DelFlg = false;
                    _unitOfWork.FollowRepository.Update(followModel);
                }
                else
                {
                    value.UserId = model.UserId;
                    value.FollowUserId = model.FollowUserId;
                    value.DelFlg = false;
                    _unitOfWork.FollowRepository.Add(value);
                }
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
                followModel.DelFlg = true;
                await _unitOfWork.SaveAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<UserFollowProfileModel> GetAllFollowingUser(string userId)
        {
            var userFollowList = _unitOfWork.FollowRepository.GetByObject(c => c.UserId == userId && c.DelFlg == false).ToList();
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
            var ApprovedPhoto = _unitOfWork.PhotoRepository.GetByObject(p => p.UserId == userId && p.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED).Count();
            var DeniedPhoto = _unitOfWork.PhotoRepository.GetByObject(p => p.UserId == userId && p.ApproveStatus == Constants.Const.PHOTO_STATUS_DENIED).Count();
            var PendingPhoto = _unitOfWork.PhotoRepository.GetByObject(p => p.UserId == userId && p.ApproveStatus == Constants.Const.PHOTO_STATUS_PENDING).Count();
            PhotoStatusStatisticModel result = new PhotoStatusStatisticModel();
            result.ApprovedPhoto = ApprovedPhoto;
            result.DeniedPhoto = DeniedPhoto;
            result.PendingPhoto = PendingPhoto;
            return result;
        }

        public IEnumerable<Dictionary<string, double>> GetSellStatisticByUserIDAndTime(StatisicModel model)
        {
            Dictionary<string, double> totalPhoto = new Dictionary<string, double>();
            Dictionary<string, double> totalAmount = new Dictionary<string, double>();
            List<Dictionary<string, double>> result = new List<Dictionary<string, double>>();
            System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
            int startMonth = DateTime.Parse(model.StartDate.ToString()).Month;
            int endMonth = DateTime.Parse(model.EndDate.ToString()).Month;
            int startYear = DateTime.Parse(model.StartDate.ToString()).Year;
            int endYear = DateTime.Parse(model.EndDate.ToString()).Year;
            DateTime startDateOfMonth = model.StartDate;
            DateTime endDateOfMonth = new DateTime(model.StartDate.Year, model.StartDate.Month, DateTime.DaysInMonth(model.StartDate.Year, model.StartDate.Month));
            bool flag = true;
            do
            {
                string month = mfi.GetAbbreviatedMonthName(startMonth);
                var count = 0;
                decimal? total = 0;
                var listOrders = _unitOfWork.OrdersRepository.GetByObject(o => o.InsDateTime >= startDateOfMonth && o.InsDateTime <= endDateOfMonth, includeProperties: "OrderDetails").ToList();
                foreach (var order in listOrders)
                {
                    var listOrderDetail = order.OrderDetails;
                    foreach (var orderDetail in listOrderDetail)
                    {
                        if (orderDetail.OwnerId == model.UserId)
                        {
                            total += orderDetail.Price;
                            count += 1;
                        }
                    }
                }
                if (startMonth == 12)
                {
                    startMonth = 1;
                } else
                {
                    startMonth++;
                }
                totalPhoto.Add(month+"-"+startYear, count);
                totalAmount.Add(month+"-"+startYear, (double)total);
                if (startMonth == endMonth && startYear == endYear)
                {
                    startDateOfMonth = startDateOfMonth.AddMonths(1);
                    startDateOfMonth = new DateTime(startDateOfMonth.Year, startDateOfMonth.Month, 1);
                    endDateOfMonth = model.EndDate;
                } else if(startMonth == 1 && startYear < endYear)
                {
                    startYear = startYear + 1;
                    startDateOfMonth = startDateOfMonth.AddMonths(1);
                    startDateOfMonth = new DateTime(startDateOfMonth.Year, startDateOfMonth.Month, 1);
                    endDateOfMonth = endDateOfMonth.AddMonths(1);
                } else
                {
                    startDateOfMonth = startDateOfMonth.AddMonths(1);
                    startDateOfMonth = new DateTime(startDateOfMonth.Year, startDateOfMonth.Month, 1);
                    endDateOfMonth = endDateOfMonth.AddMonths(1);
                }
                if (startMonth > endMonth)
                {
                    if (startYear == endYear)
                    {
                        flag = false;
                    }
                }
            } while (flag);
            result.Add(totalPhoto);
            result.Add(totalAmount);
            return result;
        }

        public bool CheckFollow(FollowModel model)
        {
            var userFollowList = _unitOfWork.FollowRepository.GetFirst(c => c.UserId == model.UserId && c.FollowUserId == model.FollowUserId).Result;
            if (userFollowList != null)
            {
                if (userFollowList.DelFlg == false)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<PhotoModelGetAll> GetAllPhotoOfAllUserWeAreFollowing(string userId)
        {
            List<PhotoModelGetAll> result = new List<PhotoModelGetAll>();
            var listOfFollowRecord = _unitOfWork.FollowRepository.GetByObject(u => u.UserId == userId).ToList();
            if(listOfFollowRecord != null)
            {
                foreach (var item in listOfFollowRecord)
                {
                    var startDateOfMonth = DateTime.Now.AddMonths(-1);
                    var listPhotoOfThatUser = _unitOfWork.PhotoRepository.GetByObject(p =>
                    p.UserId == item.FollowUserId &&
                    p.DelFlg == false &&
                    p.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED &&
                    p.DisableFlg == false &&
                    p.InsDateTime >= startDateOfMonth &&
                    p.InsDateTime <= DateTime.Now
                    ).ToList();
                    foreach (var photo in listPhotoOfThatUser)
                    {
                        var resultPhoto = _mapper.Map<PhotoModelGetAll>(photo);
                        resultPhoto.UserName = _unitOfWork.UserGenRepository.GetFirst(u => u.UserId == item.FollowUserId).Result.Username;
                        resultPhoto.UserDescription = _unitOfWork.UserGenRepository.GetFirst(u => u.UserId == item.FollowUserId).Result.Description;
                        result.Add(resultPhoto);
                    }
                }
                return result;
            }
            return null;
        }

        public IEnumerable<UserNotFollowModel> GetAllUserWeNotFollowing(string userId)
        {
            var result = new List<UserNotFollowModel>();
            var temp = new List<UserNotFollowModel>();
            temp.Add(_mapper.Map<UserNotFollowModel>(_unitOfWork.UserGenRepository.GetById(userId).Result));

            var listUserFollowing = _unitOfWork.FollowRepository.GetByObject(q => q.UserId == userId).ToList();
            foreach (var item in listUserFollowing)
            {
                temp.Add(_mapper.Map<UserNotFollowModel>(_unitOfWork.UserGenRepository.GetById(item.FollowUserId).Result));
                var listUserThatThoseUserAboveFollowing = _unitOfWork.FollowRepository.GetByObject(q => q.UserId == item.FollowUserId).ToList();
                foreach (var item2 in listUserThatThoseUserAboveFollowing)
                {
                    var test = temp.FindIndex(c => c.UserId == item2.FollowUserId);
                    if(test < 0)
                    {
                        result.Add(_mapper.Map<UserNotFollowModel>(_unitOfWork.UserGenRepository.GetById(item2.FollowUserId).Result));
                        temp.Add(_mapper.Map<UserNotFollowModel>(_unitOfWork.UserGenRepository.GetById(item2.FollowUserId).Result));
                    }
                }
            }
            return result.Take(5);
        }

        public IEnumerable<Dictionary<string, string>> CheckNotification(string userId)
        {
            var listNoti = _unitOfWork.NotificationRepository.GetByObject(c => c.UserId == userId && c.IsNotified == false).ToList();

            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            if (listNoti != null)
            {
                foreach (var noti in listNoti)
                {
                    Dictionary<string, string> item = new Dictionary<string, string>();
                    var userTemp = _unitOfWork.UsersRepository.GetById(noti.FollowUserId).Result;
                    item.Add("userId", noti.FollowUserId);
                    item.Add("username", userTemp.Username);
                    item.Add("photoId", noti.PhotoId.ToString());
                    item.Add("WMLink", noti.Wmlink);
                    item.Add("photoname", noti.PhotoName);
                    item.Add("notificationcontent", noti.NotificationContent);
                    item.Add("notificationid", noti.NotificationId.ToString());
                    result.Add(item);
                }
                return result;
            }
            return null;
        }

        public async Task<bool> DeleteNotification(int NotificationId)
        {
            var result = _unitOfWork.NotificationRepository.GetFirst(c => c.NotificationId == NotificationId).Result;
            if(result != null)
            {
                result.IsNotified = true;
                _unitOfWork.NotificationRepository.Update(result);
                await _unitOfWork.SaveAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<PhotoModel> GetAllDisablePhoto(string userId)
        {
            var listPhotoApproved = _unitOfWork.PhotoRepository.GetByObject(p => p.DelFlg == false && 
            p.DisableFlg == true && 
            p.ApproveStatus == Constants.Const.PHOTO_STATUS_APPROVED && 
            p.UserId == userId).ToList();
            if (listPhotoApproved != null)
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

   
    }
}
