using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class PhotoEdit
    {
        public int PhotoId { get; set; }
        public string PhotoName { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }

        public virtual Photo Photo { get; set; }
    }
}
