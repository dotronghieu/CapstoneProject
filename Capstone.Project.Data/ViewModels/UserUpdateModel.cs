using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class UserUpdateModel
    {
        public string FullName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DayOfBirth { get; set; }
      
    }
}
