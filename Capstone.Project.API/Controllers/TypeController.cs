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
    public class TypeController : ControllerBase
    {
        private readonly ITypeService _typeService;
        public TypeController(ITypeService typeService)
        {
            _typeService = typeService;
        }
        [HttpGet("GetAllType")]
        public IActionResult GetAllReportReason()
        {
            var result = _typeService.GetAllType();
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
