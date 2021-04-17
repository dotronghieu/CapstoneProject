using Capstone.Project.Data.Models;
using Capstone.Project.Data.Repository;
using System;
using System.Threading.Tasks;

namespace Capstone.Project.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private CapstoneProjectContext _context;

        public UnitOfWork(CapstoneProjectContext context)
        {
            _context = context;
            InitRepository();
        }

        private bool _disposed = false;

        public IGenericRepository<Category> CategoryRepository { get; set; }

        public IGenericRepository<Order> OrdersRepository { get; set; }

        public IGenericRepository<OrderDetail> OrderDetailRepository { get; set; }

        public IUserRepository UsersRepository { get; set; }

        public IGenericRepository<Role> RoleRepository { get; set; }

        public IGenericRepository<User> UserGenRepository { get; set; }

        public IGenericRepository<Photo> PhotoRepository { get; set; }
        public IGenericRepository<PhotoCategory> PhotoCategoryRepository { get; set; }
        public IGenericRepository<Report> ReportRepository { get; set; }
        public IGenericRepository<Transaction> TransactionRepository { get; set; }
        public IGenericRepository<Models.Type> TypeRepository { get; set; }
        public IGenericRepository<Follow> FollowRepository { get; set; }
        public IGenericRepository<PhotoEdit> PhotoEditRepository { get; set; }
        private void InitRepository()
        {
            CategoryRepository = new GenericRepository<Category>(_context);
            OrdersRepository = new GenericRepository<Order>(_context);
            OrderDetailRepository = new GenericRepository<OrderDetail>(_context);
            UsersRepository = new UserRepository(_context);
            RoleRepository = new GenericRepository<Role>(_context);
            UserGenRepository = new GenericRepository<User>(_context);
            PhotoRepository = new GenericRepository<Photo>(_context);
            PhotoCategoryRepository = new GenericRepository<PhotoCategory>(_context);
            ReportRepository = new GenericRepository<Report>(_context);
            TypeRepository = new GenericRepository<Models.Type>(_context);
            TransactionRepository = new GenericRepository<Transaction>(_context);
            FollowRepository = new GenericRepository<Follow>(_context);
            PhotoEditRepository = new GenericRepository<PhotoEdit>(_context);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
