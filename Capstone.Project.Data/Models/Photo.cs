﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Photo
    {
        public Photo()
        {
            Notifications = new HashSet<Notification>();
            OrderDetails = new HashSet<OrderDetail>();
            PhotoCategories = new HashSet<PhotoCategory>();
            PhotoReports = new HashSet<PhotoReport>();
            RequestDeletePhotos = new HashSet<RequestDeletePhoto>();
            Tokens = new HashSet<Token>();
        }

        public int PhotoId { get; set; }
        public string PhotoName { get; set; }
        public string Link { get; set; }
        public string Wmlink { get; set; }
        public decimal? Price { get; set; }
        public int? TypeId { get; set; }
        public string UserId { get; set; }
        public bool? DelFlg { get; set; }
        public bool? DisableFlg { get; set; }
        public DateTime? InsDateTime { get; set; }
        public string ApproveStatus { get; set; }
        public bool? IsBought { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public string Hash { get; set; }
        public string Phash { get; set; }

        public virtual Type Type { get; set; }
        public virtual User User { get; set; }
        public virtual PhotoEdit PhotoEdit { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<PhotoCategory> PhotoCategories { get; set; }
        public virtual ICollection<PhotoReport> PhotoReports { get; set; }
        public virtual ICollection<RequestDeletePhoto> RequestDeletePhotos { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }
    }
}
