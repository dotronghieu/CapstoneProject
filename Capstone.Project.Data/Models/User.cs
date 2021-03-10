using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class User
    {
        public User()
        {
            FollowFollowUsers = new HashSet<Follow>();
            FollowUsers = new HashSet<Follow>();
            Orders = new HashSet<Order>();
            PhotoReports = new HashSet<PhotoReport>();
            Photos = new HashSet<Photo>();
            RequestDeletePhotos = new HashSet<RequestDeletePhoto>();
        }

        public string UserId { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public int? RoleId { get; set; }
        public bool? DelFlg { get; set; }
        public DateTime? SuspendTime { get; set; }
        public int? ReportCounter { get; set; }
        public bool? IsVerify { get; set; }
        public string EncryptCode { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Follow> FollowFollowUsers { get; set; }
        public virtual ICollection<Follow> FollowUsers { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PhotoReport> PhotoReports { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<RequestDeletePhoto> RequestDeletePhotos { get; set; }
    }
}
