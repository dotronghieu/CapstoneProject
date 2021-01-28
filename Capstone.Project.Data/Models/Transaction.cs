using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Transaction
    {
        public string TransactionId { get; set; }
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
