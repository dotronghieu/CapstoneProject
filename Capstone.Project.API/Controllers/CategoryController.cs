using AutoMapper;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _CategoryService;
        private readonly IMapper mapper;
        public CategoryController(ICategoryService CategoryService, IMapper mapper)
        {
            _CategoryService = CategoryService;
            this.mapper = mapper;
        }
        [HttpGet()]
        public IActionResult Get()
        {
            var result = _CategoryService.GetAll(filter: p => p.DelFlg == false);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryModel model)
        {
        
            var result = await _CategoryService.CreateAsync(model);
            if (result != null)
            {             
                return Created("", mapper.Map<CategoryModel>(result));
            }
            return BadRequest(new { msg = "Duplicate Category Name"});
        }
    }
}
