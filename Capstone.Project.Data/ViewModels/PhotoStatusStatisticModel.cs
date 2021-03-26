using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.ViewModels
{
    public class PhotoStatusStatisticModel
    {
        public int NumberOfApprovedPhoto { get; set; }
        public int NumberOfPendingPhoto { get; set; }
        public int NumberOfDeniedPhoto { get; set; }
    }
}
