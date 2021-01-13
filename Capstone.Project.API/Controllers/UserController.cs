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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, [FromBody] UserUpdateModel viewModel)
        {
            var user = _userService.UpdateUser(id, viewModel);
            if (user != null)
            {
                return Created("", user);
            }
            return BadRequest();
            
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var result = await _userService.GetAllUsers();
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UserUpdatePasswordModel viewModel)
        {
            if (await _userService.CheckPasswordToUpdate(viewModel.Username, viewModel.OldPassword, viewModel.NewPassword))
            {
                return Ok(_userService.GetByUserName(viewModel.Username));
            }
            return BadRequest(new { msg = "Old Password is not correct"});
        }
    }
}
