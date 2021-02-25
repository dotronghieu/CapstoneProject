using Capstone.Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Services.IServices
{
    public interface IReportService
    {
        IEnumerable<Report> GetAllReportReason();
    }
}
