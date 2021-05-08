using AutoMapper;
using Capstone.Project.Data.Helper;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _CategoryService;
        private readonly IMapper mapper;
        public CategoryController(ICategoryService CategoryService, IMapper mapper)
        {
            _CategoryService = CategoryService; 
            this.mapper = mapper;
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet()]
        public IActionResult Get()
        {
            var result = _CategoryService.GetAll(filter: p => p.DelFlg == false);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            var result = await _CategoryService.GetByIdAsync(id);
            if(result != null)
            {
                return  Ok(result);
            }
            return BadRequest(new { msg = "Cannot found that category"});
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Roles.ROLE_ADMIN)]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Roles.ROLE_ADMIN)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryModel model)
        {

            var result = await _CategoryService.UpdateCategory(id, model);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "Cannot update category" });
        }
    }
}
