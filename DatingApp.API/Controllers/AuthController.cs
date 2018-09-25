using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.DTOs;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            this._repo = repo;
            this._config=config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTo userForRegisterDTo)
        {
            userForRegisterDTo.username=userForRegisterDTo.username.ToLower();
            if(await _repo.UserExists(userForRegisterDTo.username))
            return BadRequest("User Already Exists.");

            var userToCreate=new User
            {
             Username=userForRegisterDTo.username

            };
            var createdUser=await _repo.Register(userToCreate,userForRegisterDTo.password);
            return StatusCode(201);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
            var userFromRepo=await _repo.Login(userForLoginDTO.username.ToLower(),userForLoginDTO.password);
            if(userFromRepo==null)
            return Unauthorized();

            var claims=new[] {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };

            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds= new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor= new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(1),
                SigningCredentials=creds

            };
            var tokenHandler=new JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new{
                token=tokenHandler.WriteToken(token)
            });

        }
    }
}