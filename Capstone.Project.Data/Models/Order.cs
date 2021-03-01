using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public string OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime? InsDateTime { get; set; }
        public string TransactionId { get; set; }

        public virtual User User { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
