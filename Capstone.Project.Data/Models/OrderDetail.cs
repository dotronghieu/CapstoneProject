using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class OrderDetail
    {
        public string OrderId { get; set; }
        public int PhotoId { get; set; }
        public double? Price { get; set; }

        public virtual Order Order { get; set; }
        public virtual Photo Photo { get; set; }
    }
}
