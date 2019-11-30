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
            var operationStatusCode = User.Validate(userName, userPassword);
            if (operationStatusCode == "200")
            {                
                 return RedirectToAction("Index", "Chats");
            }
            else
            {
                ModelState.AddModelError(string.Empty, operationStatusCode);
                return View();
            }
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

            if (operationStatusCode == "200")
            {                
                 return RedirectToAction("Index", "Chats");
            }
            else
            {
                ModelState.AddModelError(string.Empty, operationStatusCode);
                return View(User.GetQuestions());
            }
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {            
            return View(User.GetQuestions());
        }
        [HttpPost]
        public ActionResult ChangePassword(string userName, string newPassword, string questionSelected, string secretAnswer)
        {
            var operationStatusCode = User.ModifyPassword(userName, questionSelected, secretAnswer, newPassword);
            ModelState.AddModelError(string.Empty, operationStatusCode);            
            return View(User.GetQuestions());
        }

        public ActionResult DeleteAccout(string pass)
        {
            HttpCookie objRequestRead= Request.Cookies["auth"];
            Autentication JWT = new Autentication();
            if (objRequestRead!= null && objRequestRead["jwt"]!= null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {
                string loggedUser = objRequestRead["username"];
                if (User.DeleteMyAccount(loggedUser, pass).Equals("Usuario eliminado correctamente"))
                {
                    var deleteMyMessages = new MessagesLogic();
                    var IAmDone = deleteMyMessages.DeleteMessages(loggedUser);
                    objRequestRead.Values["jwt"] = "400";
                    Response.Cookies.Add(objRequestRead);
                    return RedirectToAction("LogInUser");
                }
                else
                {
                    return RedirectToAction("Index", "Chats"); 
                }
                
            }
            else
            {
                return RedirectToAction("Error", "Chats");
            }
        }

        public ActionResult LogOut()
        {
            HttpCookie objRequestRead= Request.Cookies["auth"];
            Autentication JWT = new Autentication();
            if (objRequestRead!= null && objRequestRead["jwt"]!= null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {

                    objRequestRead.Values["jwt"] = "400";
                    Response.Cookies.Add(objRequestRead);
                    return RedirectToAction("LogInUser");               
            }
            else
            {
                return RedirectToAction("Error", "Chats");
            }
        }
    }
}