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
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        [HttpGet("GetTransaction/{id}")]
        public IActionResult GetTransaction(string id)
        {
            var result = _transactionService.GetTransaction(id);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new { msg = "No transaction recorded" });
        }
        [HttpGet("GetAllTransactionByUserID/{userID}")]
        public IActionResult GetTransactionByUserId(string userID)
        {
            var result =  _transactionService.GetAllTransactionByUserId(userID);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new { msg = "No transaction recorded" });
        }
    }
}
