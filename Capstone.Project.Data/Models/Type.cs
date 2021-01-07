using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Type
    {
        public Type()
        {
            Photos = new HashSet<Photo>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public bool? DelFlg { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
    }
}
