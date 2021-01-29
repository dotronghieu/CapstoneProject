using Capstone.Project.Data.Helper;
using Capstone.Project.Data.Models;
using Capstone.Project.Data.ViewModels;
using Capstone.Project.Services.IServices;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;



namespace Capstone.Project.API.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        private readonly IConfiguration _config;

        public AuthController(IUserService userService, IRoleService roleService, IConfiguration config)
        {
            _userService = userService;
            _roleService = roleService;
            _config = config;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _userService.CreateUser(model, model.Password);
            if (result != null)
            {
                return Created("", result);
            }
            return BadRequest(new { msg = "Username already taken"});
        }
        [HttpGet("Verify/{id}")]
        public async Task<IActionResult> Verify(string id)
        {
            var result = await _userService.Activate(id);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost("RequestVerify")]
        public async Task<IActionResult> RequestVerify([FromBody] RequestEmailModel model)
        {
            var result = await _userService.RequestVerify(model);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userService.GetByUserName(model.Username);
            var result = await _userService.CheckPassWord(model.Username, model.Password);
            if (user != null && result)
            {
                var _role = _roleService.GetRole(user);

                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, _role)
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
                var firebaseProject = _config.GetSection("AppSettings:FirebaseProject").Value;
                var token = new JwtSecurityToken(
                    issuer: "https://securetoken.google.com/" + firebaseProject,
                    audience: firebaseProject,
                    expires: DateTime.Now.AddYears(13),
                    claims: authClaims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token).ToString(),
                    role = _role,
                    email = user.Email,
                    fullName = user.FullName,
                    username = user.Username,
                    avatar = user.Avatar,
                    isVerified = user.IsVerify,
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
        [HttpPost("Google")]
        public async Task<IActionResult> LoginGoogle(UserModelRequestParam login)
        {
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(login.Token);
            if (decodedToken != null)
            {
                string uid = decodedToken.Uid;
                UserModel user = await _userService.LoginGoogle(uid, login.Username, login.Password);


                var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Role, Constants.Roles.ROLE_USER)
                    };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
                var firebaseProject = _config.GetSection("AppSettings:FirebaseProject").Value;
                var token = new JwtSecurityToken(
                    issuer: "https://securetoken.google.com/" + firebaseProject,
                    audience: firebaseProject,
                    expires: DateTime.Now.AddYears(13),
                    claims: authClaims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    role = Constants.Roles.ROLE_USER,
                    email = user.Email,
                    fullName = user.FullName,
                    username = user.Username,
                    avatar = user.Avatar,
                    expiration = token.ValidTo
                });
            }
            return BadRequest();
        }
    }
}
