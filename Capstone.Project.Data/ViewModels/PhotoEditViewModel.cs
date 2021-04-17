using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class PhotoEditViewModel
    {
        public int PhotoId { get; set; }
        public string PhotoName { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
    }
}
