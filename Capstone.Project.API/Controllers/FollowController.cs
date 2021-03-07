﻿using Capstone.Project.Data.ViewModels;
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
    public class FollowController : ControllerBase
    {
        private readonly IUserService _userService;
        public FollowController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Follow")]
        public async Task<IActionResult> Follow([FromBody] FollowModel model)
        {
            var result = await _userService.Follow(model);
            if (result)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "Follow Failed" });
        }
        [HttpPost("UnFollow")]
        public async Task<IActionResult> UnFollow([FromBody] FollowModel model)
        {
            var result = await _userService.Unfollow(model);
            if (result)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "UnFollow Failed" });
        }
        [HttpGet("GetFollowingUser/{id}")]
        public IActionResult Get(string id)
        {
            var result =  _userService.GetAllFollowingUser(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "There is no following user" });
        }
        [HttpGet("GetFollowingUserInfo/{id}")]
        public async Task<IActionResult> GetFollowingUserInfo(string id)
        {
            var result = await _userService.GetProfileByID(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
