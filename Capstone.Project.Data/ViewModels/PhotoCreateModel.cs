using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class PhotoCreateModel
    {
        public string PhotoName { get; set; }
        public IFormFile File { get; set; }
        public decimal? Price { get; set; }
        public int? TypeId { get; set; }
        public string UserId { get; set; }
        public List<int> ListCategory { get; set; }
        public string Description { get; set; }
    }
}
