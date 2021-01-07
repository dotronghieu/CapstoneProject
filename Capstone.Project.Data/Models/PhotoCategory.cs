using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class PhotoCategory
    {
        public int CategoryId { get; set; }
        public int PhotoId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Photo Photo { get; set; }
    }
}
