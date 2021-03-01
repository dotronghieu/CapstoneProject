using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Transaction
    {
        public string TransactionId { get; set; }
        public DateTime? CreateTime { get; set; }
        public decimal? Amount { get; set; }
        public string PayerId { get; set; }
        public string PayerPaypalEmail { get; set; }

        public virtual Order TransactionNavigation { get; set; }
    }
}
