﻿using Capstone.Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class PhotoModelAdmin
    {
        public int PhotoId { get; set; }
        public string PhotoName { get; set; }
        public string Link { get; set; }
        public string Wmlink { get; set; }
        public decimal? Price { get; set; }
        public int? TypeId { get; set; }
        public string UserId { get; set; }
        public string ApproveStatus { get; set; }
        public string Note { get; set; }
        public bool? DelFlg { get; set; }
        public string Description { get; set; }
        public string Phash { get; set; }
        public virtual PhotoModel SimilarPhoto { get; set; }
        public virtual ICollection<CategoryModel> Category { get; set; }
        public DateTime? InsDateTime { get; set; }
    }
}
