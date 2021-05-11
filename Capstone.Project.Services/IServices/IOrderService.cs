using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Project.Services.IServices
{
    public interface IOrderService
    {
        public Task<IEnumerable<PhotoTransactionModel>> GetUserBoughtPhoto(string id);
        public Task<Order> OrderPhoto(OrderModel orderModel);
        public string ConvertToBinary(ulong hash);

    }
}
