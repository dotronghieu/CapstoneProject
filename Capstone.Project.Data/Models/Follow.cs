using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Follow
    {
        public string UserId { get; set; }
        public string FollowUserId { get; set; }

        public virtual User FollowUser { get; set; }
        public virtual User User { get; set; }
    }
}
