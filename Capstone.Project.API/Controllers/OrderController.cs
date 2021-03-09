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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost()]
        public async Task<IActionResult> OrderPhoto([FromForm] OrderModel orderModel)
        {
            var result = await _orderService.OrderPhoto(orderModel);
            if (result != null)
            {
                return Created("", result);
            }

            return BadRequest();
        }
    }
}
