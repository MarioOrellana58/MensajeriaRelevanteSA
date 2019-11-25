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
            List<string> questionsForUsers = new List<string>()
            {
                "¿ Cuál es el nombre de tu personaje favorito ?",
                "¿ Cuál es el nombre de tu mascota ?",
                "¿ Cuál es el nombre de tu familiar mas querido ?",
                "¿Cuál es tu comida favorita ?"
            };
            return View(questionsForUsers);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(string userName, string userPassword, string userQuestion, string userAnswer)
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
            List<string> questionsForUsers = new List<string>()
            {
                "¿ Cuál es el nombre de tu personaje favorito ?",
                "¿ Cuál es el nombre de tu mascota ?",
                "¿ Cuál es el nombre de tu familiar mas querido ?",
                "¿Cuál es tu comida favorita ?"
            };
            return View(questionsForUsers);
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            List<string> questionsForUsers = new List<string>()
            {
                "¿ Cuál es el nombre de tu personaje favorito ?",
                "¿ Cuál es el nombre de tu mascota ?",
                "¿ Cuál es el nombre de tu familiar mas querido ?",
                "¿Cuál es tu comida favorita ?"
            };
            return View(questionsForUsers);
        }
        [HttpPost]
        public ActionResult ChangePassword(string userName, string newPassword, string questionSelected, string secretAnswer)
        {
            List<string> questionsForUsers = new List<string>()
            {
                "¿ Cuál es el nombre de tu personaje favorito ?",
                "¿ Cuál es el nombre de tu mascota ?",
                "¿ Cuál es el nombre de tu familiar mas querido ?",
                "¿Cuál es tu comida favorita ?"
            };
            return View(questionsForUsers);
        }
    }
}