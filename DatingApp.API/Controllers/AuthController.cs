using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.DTOs;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            this._repo = repo;
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
    }
}