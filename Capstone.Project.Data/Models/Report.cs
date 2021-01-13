using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class Report
    {
        public Report()
        {
            PhotoReportDetails = new HashSet<PhotoReportDetail>();
        }

        public int ReportId { get; set; }
        public string ReportReason { get; set; }

        public virtual ICollection<PhotoReportDetail> PhotoReportDetails { get; set; }
    }
}
