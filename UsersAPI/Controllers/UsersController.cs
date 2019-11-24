using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Models;

namespace Pizza_API.Controllers
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

        [HttpGet]
        public ActionResult<List<UserNode>> Get() =>
            _userService.Get();

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
            _userService.Create(user);

            return CreatedAtRoute("GetUser", new { Username = user.Username.ToString() }, user);
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

        [HttpDelete("{Username}")]
        public IActionResult Delete(string username)
        {
            var user = _userService.Get(username);

            if (user == null)
            {
                return StatusCode(404, "Your user does not exist");
            }

            _userService.Remove(user.Username);

            return StatusCode(200, "Your user has been deleted :D");
        }
    }
}