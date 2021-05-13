using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class PhotoModelGetAll
    {
        public int PhotoId { get; set; }
        public string PhotoName { get; set; }
        public string Link { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserDescription { get; set; }
        public string Description { get; set; }
        public decimal? Phash { get; set; }
        public int? TypeId { get; set; }
        public virtual ICollection<CategoryModel> Category { get; set; }
        public string Wmlink { get; set; }
    }
}
