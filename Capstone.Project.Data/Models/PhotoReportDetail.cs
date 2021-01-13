using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class PhotoReportDetail
    {
        public int PhotoReportId { get; set; }
        public int ReportId { get; set; }

        public virtual PhotoReport PhotoReport { get; set; }
        public virtual Report Report { get; set; }
    }
}
