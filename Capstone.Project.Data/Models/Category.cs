using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Category
    {
        public Category()
        {
            PhotoCategories = new HashSet<PhotoCategory>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool? DelFlg { get; set; }

        public virtual ICollection<PhotoCategory> PhotoCategories { get; set; }
    }
}
