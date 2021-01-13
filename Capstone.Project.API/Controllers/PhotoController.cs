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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IMapper mapper;
        public PhotoController(IPhotoService categoryService, IMapper mapper)
        {
            _photoService = categoryService;
            this.mapper = mapper;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            var result = _photoService.GetAll(filter: p => p.DelFlg == false, includeProperties: "PhotoCategories");
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var photo = await _photoService.GetByIdAsync(id);
            if(photo != null)
            {
                return Ok(photo);
            }
            return BadRequest( new { msg = "photo is not found"});
        }
    }
}
