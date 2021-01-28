using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class RequestDeletePhoto
    {
        public string UserId { get; set; }
        public int PhotoId { get; set; }
        public bool? IsApprove { get; set; }

        public virtual Photo Photo { get; set; }
        public virtual User User { get; set; }
    }
}
