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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        public UserController(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
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
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await  _userService.GetByID(id);
            if (user != null)
            {
                return Ok(user);
            }
            return BadRequest(new {msg = "No user found" });

        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var result = await _userService.GetAllUsers();
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpPut]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UserUpdatePasswordModel viewModel)
        {
            if (await _userService.CheckPasswordToUpdate(viewModel.Username, viewModel.OldPassword, viewModel.NewPassword))
            {
                return Ok(_userService.GetByUserName(viewModel.Username).Result);
            }
            return BadRequest(new { msg = "Old Password is not correct"});
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_ADMIN)]
        [HttpPut("ApprovePhoto/{id}")]
        public async Task<IActionResult> ApprovePhoto(int id)
        {
            if (await _userService.ApprovePhoto(id))
            {
                return Ok(new { msg = "Photo has been approved"});
            }
            return BadRequest(new { msg = "Invalid PhotoID" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_ADMIN)]
        [HttpPut("DeniedPhoto")]
        public async Task<IActionResult> DeniedPhoto([FromBody] DeniedPhotoModel model)
        {
            if (await _userService.DeniedPhoto(model))
            {
                return Ok(new { msg = "Photo has been denied" });
            }
            return BadRequest(new { msg = "Invalid PhotoID" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpGet("GetUserApprovedPhoto/{id}")]
        public  IActionResult GetUserApprovedPhoto(string id)
        {
            var result =  _userService.GetAllPhotoApproved(id);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new { msg = "No photo recorded" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpGet("GetUserPendingPhoto/{id}")]
        public IActionResult GetUserPendingPhoto(string id)
        {
            var result = _userService.GetAllPendingPhoto(id);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new { msg = "No photo recorded" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpGet("GetUserDeniedPhoto/{id}")]
        public IActionResult GetUserDeniedPhoto(string id)
        {
            var result = _userService.GetAllDeniedPhoto(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "No photo recorded" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpGet("GetUserNormalPhoto/{id}")]
        public IActionResult GetUserNormalPhoto(string id)
        {
            var result = _userService.GetAllNormalPhotoApproved(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "No photo recorded" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpGet("GetUserExclusivePhoto/{id}")]
        public IActionResult GetUserExclusivePhoto(string id)
        {
            var result = _userService.GetAllExclusivePhotoApproved(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "No photo recorded" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpGet("GetUserExclusivePropertyPhoto/{id}")]
        public IActionResult GetUserExclusivePropertyPhoto(string id)
        {
            var result = _userService.GetAllExclusivePropertyPhotoApproved(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "No photo recorded" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpGet("GetPhotoStatusStatistic/{id}")]
        public IActionResult GetPhotoStatusStatistic(string id)
        {
            var result = _userService.GetPhotoStatusStatisticByUserID(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpPost("GetSellPhotoStatistic")]
        public IActionResult GetSellPhotoStatistic([FromBody] StatisicModel model)
        {
            var result = _userService.GetSellStatisticByUserIDAndTime(model);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new { msg = "No record" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpGet("GetNotification/{userid}")]
        public IActionResult GetNotification(string userid)
        {
            var result = _userService.CheckNotification(userid);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new { msg = "No new notification" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Const.ROLE_USER)]
        [HttpPut("DeleteNotification/{NotificationId}")]
        public async Task<IActionResult> DeleteNotification(int NotificationId)
        {
            var result = await _userService.DeleteNotification(NotificationId);
            if (result == true)
            {
                return Ok(new { msg = "Notification has been deleted" });
            }
            return BadRequest();
        }
    }
}
