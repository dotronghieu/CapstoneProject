using Capstone.Project.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.IServices
{
    public interface IOrderService
    {
        public IEnumerable<PhotoModel> GetUserBoughtPhoto(string id);
    }
}
