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
            var dechipherData = new SDES();

            user.Password = dechipherData.DecipherText(user.Password);
            user.Question = dechipherData.DecipherText(user.Question);
            user.Answer = dechipherData.DecipherText(user.Answer);

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
            var cipherData = new SDES();

            user.Password = cipherData.CipherText(user.Password);
            user.Question = cipherData.CipherText(user.Question);
            user.Answer = cipherData.CipherText(user.Answer);

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
            var cipherData = new SDES();
            userIn.Password = cipherData.CipherText(userIn.Password);
            userIn.Id = user.Id;
            _userService.Update(username, userIn);

            return StatusCode(200, "Your user has been updated :D");
        }

    }
}