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
        public ActionResult CreateUser(string userName, string userPassword)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:51209/api/Users");
                var user = new UserNode() { Username = userName, Password = userPassword };

                var postTask = client.PostAsJsonAsync<UserNode>("Users", user);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Usuario insertado");
                }
                else
                {
                    if ((int)result.StatusCode == 409)
                    {
                        ModelState.AddModelError(string.Empty, "409 conflicto. El usuario seleccionado ya existe, intente otro");
                        
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "500 error del servidor. Contacte a un desarrollador del sistema");

                    }
                }
            }
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