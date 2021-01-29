using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        public UserController(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
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
        [HttpGet("GetBoughtPhoto/{id}")]
        public async Task<IActionResult> GetBoughtPhoto(string id)
        {
            var result = await _orderService.GetUserBoughtPhoto(id);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new { msg = "No order recorded" });
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
