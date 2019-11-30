using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Models;
using AlternativeProcesses;
namespace UsersAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _userService;

        public UsersController(UsersService usersService)
        {
            _userService = usersService;
        }

        [HttpGet("{Username}", Name = "GetUser")]
        public ActionResult<UserNode> Get(string username)
        {
            var user = _userService.Get(username);
            
            if (user == null)
            {
                return StatusCode(404, "Your user does not exist");
            }

            return user;
        }

        [HttpPost]
        public ActionResult<UserNode> Create(UserNode user)
        {
            var findUser = _userService.Get(user.Username);

            if (findUser != null)
            {
                return StatusCode(409, "That username already exists :( try another one");
            }

            _userService.Create(user);

            return StatusCode(200, "Your user has been created :D");
        }

        [HttpPut("{Username}")]
        public IActionResult Update(string username, UserNode userIn)
        {
            var user = _userService.Get(username);
            if (user == null)
            {
                return NotFound();
            }
            userIn.Id = user.Id;
            _userService.Update(username, userIn);

            return StatusCode(200, "Your user has been updated :D");
        }

        [HttpDelete("{userName}")]
        public IActionResult Delete(string userName)
        {
            var user = _userService.Get(userName);

            if (user == null)
            {
                return StatusCode(404, "El usuario no existe");
            }

            _userService.Remove(user.Username);

            return StatusCode(200, "Usuario eliminado :D");
        }
    }
}