using AutoMapper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.UnitOfWork;
using Capstone.Project.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Report> GetAllReportReason()
        {
            return _unitOfWork.ReportRepository.GetAll();
        }
    }
}
