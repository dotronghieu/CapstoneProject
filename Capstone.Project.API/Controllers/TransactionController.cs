﻿using Capstone.Project.Services.IServices;
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
        public async Task<IActionResult> GetBoughtPhoto(string id)
        {
            var result = await _transactionService.GetTransaction(id);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new { msg = "No transaction recorded" });
        }
    }
}