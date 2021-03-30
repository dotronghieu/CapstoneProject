using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class PhotoStatusStatisticModel
    {
        public int ApprovedPhoto { get; set; }
        public int PendingPhoto { get; set; }
        public int DeniedPhoto { get; set; }
    }
}
