using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using MensajesRelevantesSA.Models;
using MongoDB.Driver.Communication;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using MensajesRelevantesSA.Repository;

namespace MensajesRelevantesSA.Controllers
{
    public class UsersController : Controller
    {

        private UsersLogic User = new UsersLogic();

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
            return View(User.GetQuestions());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(string userName, string userPassword, string userQuestion, string userAnswer)
        {
            var operationStatusCode = User.Create(userName, userPassword, userAnswer, userQuestion);            
            ModelState.AddModelError(string.Empty, operationStatusCode);
            return View(User.GetQuestions());
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {            
            return View(User.GetQuestions());
        }
        [HttpPost]
        public ActionResult ChangePassword(string userName, string newPassword, string questionSelected, string secretAnswer)
        {
            return View(User.GetQuestions());
        }
    }
}