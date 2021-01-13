using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public  class UserModel
    {
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string UserKey { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public int RoleId { get; set; }
    }
}
