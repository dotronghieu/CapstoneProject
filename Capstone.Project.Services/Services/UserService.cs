using AutoMapper;
using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.Services
{
    public class UserService : IUserService
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
            var user = _mapper.Map<User>(model);
            await _unitOfWork.UsersRepository.Create(user, password);
            await _unitOfWork.SaveAsync();
            return user;
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

      
    }
}
