using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MensajesRelevantesSA.Models;
namespace MensajesRelevantesSA.Controllers
{
    public class UsersController : Controller
    {
        [HttpGet]
        public ActionResult LogInUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogInUser(string userName, string userPassword)
        {
            return View();
        }
        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateUser(string newName, string newPass)
        {
            return View();
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string userName)
        {
            return View();
        }
    }
}