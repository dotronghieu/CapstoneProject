using Capstone.Project.Data.Models;
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

    }
}
