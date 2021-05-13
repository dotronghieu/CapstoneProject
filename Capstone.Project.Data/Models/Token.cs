using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Token
    {
        public string TokenId { get; set; }
        public string UserId { get; set; }
        public int? PhotoId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? NumberOfUses { get; set; }

        public virtual Photo Photo { get; set; }
        public virtual User User { get; set; }
    }
}
