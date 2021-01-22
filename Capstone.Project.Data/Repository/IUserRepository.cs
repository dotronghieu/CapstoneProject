using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capstone.Project.Data.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetByUsername(string username);
        Task<User> Create(User user, string password);
        void Delete(User user);
        bool Update(User user);
        Task<User> GetById(string id);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    }
}
