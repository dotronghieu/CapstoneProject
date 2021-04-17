using Capstone.Project.Data.Models;
using Capstone.Project.Data.Repository;
using System.Threading.Tasks;

namespace Capstone.Project.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Order> OrdersRepository { get; }
        IGenericRepository<OrderDetail> OrderDetailRepository { get; }
        IUserRepository UsersRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<User> UserGenRepository { get; }
        IGenericRepository<Photo> PhotoRepository { get; }
        IGenericRepository<PhotoCategory> PhotoCategoryRepository { get; }
        IGenericRepository<Report> ReportRepository { get; }
        IGenericRepository<Type> TypeRepository { get; }
        IGenericRepository<Transaction> TransactionRepository { get; }
        IGenericRepository<Follow> FollowRepository { get; }
        IGenericRepository<PhotoEdit> PhotoEditRepository { get; }
        Task<int> SaveAsync();
    }
}
