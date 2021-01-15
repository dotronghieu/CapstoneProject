using AutoMapper;
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
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IPhotoCategoryService _photoCategoryService;
        private readonly IMapper mapper;
        public PhotoController(IPhotoService categoryService, IMapper mapper, IPhotoCategoryService photoCategoryService)
        {
            _photoService = categoryService;
            this.mapper = mapper;
            _photoCategoryService = photoCategoryService;
        }
        [AllowAnonymous]
        [HttpGet()]
        public IActionResult Get()
        {
            var list = _photoService.GetAll(filter: p => p.DelFlg == false, includeProperties: "PhotoCategories");
            if(list != null)
            {
                List<PhotoModelGetAll> result = new List<PhotoModelGetAll>();
                foreach (var item in list)
                {
                    result.Add(mapper.Map<PhotoModelGetAll>(item));
                }
                return Ok(result);
            }
            return BadRequest(new { msg = "not found any photo" });
        }
        [AllowAnonymous]
        [HttpGet("random")]
        public IActionResult GetRandom()
        {
            var list = _photoService.GetRandomPhoto();
            if(list != null)
            {
                return Ok(list);
            }
            return BadRequest(new { msg = "Empty List" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [AllowAnonymous]
        [HttpGet("getByCategory/{id}")]
        public IActionResult GetPhotoByCategory(int id)
        {
            var photo = _photoCategoryService.GetPhotoByCategory(id);
            if (photo != null)
            {
                return Ok(photo.ToList());
            }
            return BadRequest();
        }
        [AllowAnonymous]
        [HttpGet("getCategoryByPhoto/{id}")]
        public IActionResult GetCategoryByPhoto(int id)
        {
            var category = _photoCategoryService.GetCategoryByPhoto(id);
            if (category != null)
            {
                return Ok(category);
            }
            return BadRequest();
        }
    }
}
