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
using System.Net.Http;
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
        private readonly IPhotoUploadDownloadService _photoUploadDownloadService;
        public PhotoController(IPhotoService categoryService, IMapper mapper, IPhotoCategoryService photoCategoryService, IPhotoUploadDownloadService photoUploadDownloadService)
        {
            _photoService = categoryService;
            this.mapper = mapper;
            _photoCategoryService = photoCategoryService;
            _photoUploadDownloadService = photoUploadDownloadService;
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
            if (list != null)
            {
                return Ok(list);
            }
            return BadRequest(new { msg = "Empty List" });
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [AllowAnonymous]
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
                return Ok(photo);
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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Roles.ROLE_USER)]
        [HttpPut("{id}")]
        public IActionResult UserUpdatePhoto(int id, [FromBody] PhotoModel model)
        {
            var result =  _photoService.UpdatePhoto(id, model);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "Photo Update Fail" });
        }
        //[HttpPut]
        //public IActionResult EncryptPhoto()
        //{
        //     _photoService.EncryptAllPhoto();         
        //     return Ok();
         
        //}
        [AllowAnonymous]
        [HttpPost("CreatePhoto")]
        public async Task<IActionResult> CreatePhoto([FromBody] PhotoCreateModel model)
        {
            var result = await _photoUploadDownloadService.CreatePhoto(model);
            if (result != null)
            {
                return Created("", result);
            }

            return BadRequest(new { msg = "Photo upload failed" });
        }
        [AllowAnonymous]
        [HttpGet("DownloadPhoto/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            string url = _photoUploadDownloadService.DownloadPhoto(id);
            if(url != null)
            {
                using var httpClient = new HttpClient();
                byte[] imageBytes = await httpClient.GetByteArrayAsync(url);
                httpClient.Dispose();
                char[] charArray = url.ToCharArray();
                Array.Reverse(charArray);
                string rurl = new string(charArray);
                rurl = rurl.Substring(53, 6);
                int i = rurl.IndexOf('.');
                rurl = rurl.Substring(0, i);
                charArray = rurl.ToCharArray();
                Array.Reverse(charArray);
                rurl = new string(charArray);
                return File(imageBytes, "image/*", "image." + rurl);
            }
            return BadRequest();
        }
        [AllowAnonymous]
        [HttpPut("DeletePhoto/{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            bool result = await _photoUploadDownloadService.DeletePhoto(id);
            if(result)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
