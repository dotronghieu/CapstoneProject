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
                return Ok(_userService.GetByUserName(viewModel.Username).Result);
            }
            return BadRequest(new { msg = "Old Password is not correct"});
        }
       
        [HttpPut("ApprovePhoto/{id}")]
        public async Task<IActionResult> ApprovePhoto(int id)
        {
            if (await _userService.ApprovePhoto(id))
            {
                return Ok(new { msg = "Photo has been approved"});
            }
            return BadRequest(new { msg = "Invalid PhotoID" });
        }
        [HttpPut("DeniedPhoto")]
        public async Task<IActionResult> DeniedPhoto([FromBody] DeniedPhotoModel model)
        {
            if (await _userService.DeniedPhoto(model))
            {
                return Ok(new { msg = "Photo has been denied" });
            }
            return BadRequest(new { msg = "Invalid PhotoID" });
        }
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
