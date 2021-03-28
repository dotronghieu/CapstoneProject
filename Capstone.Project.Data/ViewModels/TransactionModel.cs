using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class TransactionModel
    {
        public string TransactionId { get; set; }
        public DateTime? CreateTime { get; set; }
        public decimal? Amount { get; set; }
        public string PayerId { get; set; }
        public string PayerPaypalEmail { get; set; }
    }
}
