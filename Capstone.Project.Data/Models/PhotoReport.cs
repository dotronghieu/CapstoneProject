using System;
using System.Collections.Generic;

#nullable disable

namespace Capstone.Project.Data.Models
{
    public partial class PhotoReport
    {
        public PhotoReport()
        {
            PhotoReportDetails = new HashSet<PhotoReportDetail>();
        }

        public int PhotoReportId { get; set; }
        public string UserId { get; set; }
        public int? PhotoId { get; set; }
        public string Description { get; set; }

        public virtual Photo Photo { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<PhotoReportDetail> PhotoReportDetails { get; set; }
    }
}
