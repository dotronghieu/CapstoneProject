using Capstone.Project.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Project.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet("GetAllReportReason")]
        public IActionResult GetAllReportReason()
        {
            var result = _reportService.GetAllReportReason();
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
