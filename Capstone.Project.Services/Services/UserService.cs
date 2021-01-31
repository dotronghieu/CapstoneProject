using AutoMapper;
using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using FirebaseAdmin.Auth;
using System;
using System.Collections.Generic;
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
                user.RoleId = Constants.Roles.ROLE_USER_ID;
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
                if (action == "Login")
                {
                    return null;
                }
                throw new AppException("Cannot find " + username);
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
                    RoleId = Constants.Roles.ROLE_USER_ID,
                    Email = user_firebase.Email,
                    FullName = user_firebase.DisplayName,
                    IsVerify = true,
                    DelFlg = false,
                    Avatar = user_firebase.PhotoUrl
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
                var verifyUrl = "https://capstonerestapi.azurewebsites.net/api/v1/Auth/Verify/" + model.UserId;
                var fromMail = new MailAddress(Constants.Roles.IMAGO_EMAIL, "Imago (No Reply)");
                var toMail = new MailAddress(model.Email);
                var imagoPassword = Constants.Roles.IMAGO_EMAIL_PASSWORD;
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
    }
}
