using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            Photos = new HashSet<Photo>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string UserKey { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public int? RoleId { get; set; }
        public bool? DelFlg { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
