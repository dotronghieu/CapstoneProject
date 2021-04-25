using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public string UserId { get; set; }
        public string FollowUserId { get; set; }
        public int? PhotoId { get; set; }
        public string NotificationContent { get; set; }
        public string Wmlink { get; set; }
        public string PhotoName { get; set; }
        public bool? IsNotified { get; set; }

        public virtual Follow Follow { get; set; }
        public virtual Photo Photo { get; set; }
    }
}
