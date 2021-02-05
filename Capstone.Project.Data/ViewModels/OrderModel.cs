using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class OrderModel
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public double Total { get; set; }
        public List<OrderDetailModel> OrderDetail { get; set; }
    }
}
