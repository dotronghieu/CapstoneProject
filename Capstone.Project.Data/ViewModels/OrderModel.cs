using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class OrderModel
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public string TransactionId { get; set; }
        public DateTime? CreateTime { get; set; }
        public decimal? Amount { get; set; }
        public string PayerId { get; set; }
        public string PayerPaypalEmail { get; set; }
        public List<OrderDetailModel> OrderDetail { get; set; }
    }
}
