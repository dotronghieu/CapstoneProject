using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Photo
    {
        public Photo()
        {
            OrderDetails = new HashSet<OrderDetail>();
            PhotoCategories = new HashSet<PhotoCategory>();
        }

        public int PhotoId { get; set; }
        public string PhotoName { get; set; }
        public string Link { get; set; }
        public decimal? Price { get; set; }
        public int? TypeId { get; set; }
        public int? UserId { get; set; }
        public bool? DelFlg { get; set; }
        public DateTime? InsDateTime { get; set; }

        public virtual Type Type { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<PhotoCategory> PhotoCategories { get; set; }
    }
}
