﻿using MensajesRelevantesSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace MensajesRelevantesSA.Repository
{
    public class UsersLogic
    {
        public List<string> GetQuestions()
        {
            return new List<string>()
            {
                "¿ Cuál es el nombre de tu personaje favorito ?",
                "¿ Cuál es el nombre de tu mascota ?",
                "¿ Cuál es el nombre de tu familiar mas querido ?",
                "¿Cuál es tu comida favorita ?"
            };
        }

        public string Create(string userName, string userPassword, string userAnswer, string userQuestion)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:51209/api/Users");
                var user = new UserNode() { Username = userName, Password = userPassword, Answer = userAnswer, Question = userQuestion};

                var postTask = client.PostAsJsonAsync("Users", user);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return "200. Usuario insertado :D";
                }
                else
                {
                    if ((int)result.StatusCode == 409)
                    {
                        return "409 conflicto. El usuario seleccionado ya existe, intente otro :v";
                        
                    }
                    else if ((int)result.StatusCode >= 400 && (int)result.StatusCode < 500)
                    {
                        return result.StatusCode.ToString() + ". Revise los datos ingresados :D";
                    }
                    else
                    {
                        return result.StatusCode.ToString() + ". Contacte a un desarrollador del sistema D:";
                    }
                }
            }
        }

        dynamic getUserByUsername(string userName)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://localhost:51209/api/Users");
                UserNode searchedUser = null;
                var responseTask = client.GetAsync("Users/" + userName);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<UserNode>();
                    readTask.Wait();
                    searchedUser = readTask.Result;
                    return searchedUser;
                }
                else
                {

                    searchedUser = null;
                    if ((int)result.StatusCode == 404)
                    {
                        return "404 no encontrado. El usuario que has ingresado no existe :(";
                    }
                    else
                    {
                        return result.StatusCode.ToString() + ". Contacte a un desarrollador del sistema D:";
                    }
                }
            }
        }

        public string ModifyPassword(string userName, string secretQuestion, string secretAnswer, string newPassword)
        {
            using (var client = new HttpClient())
            {
                var searchedUser = getUserByUsername(userName);

                if (searchedUser.GetType().ToString() == "string")
                {
                    return searchedUser;
                }

                client.BaseAddress = new Uri("http://localhost:51209/api/Users");

                if (searchedUser.Question != secretQuestion || searchedUser.Answer != secretAnswer)
                {
                    return "Tu pregunta y o respuesta no son válidas >:(";
                }

                var updatedUser = new UserNode() { Username = userName, Password = newPassword, Answer = secretAnswer, Question = secretAnswer };

                var putTask = client.PutAsJsonAsync("Users", updatedUser);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return "200. Contraseña actualizada :D";
                }
                else
                {

                    if ((int)result.StatusCode >= 400 && (int)result.StatusCode < 500)
                    {
                        return result.StatusCode.ToString() + ". Revise los datos ingresados :D";
                    }
                    else
                    {
                        return result.StatusCode.ToString() + ". Contacte a un desarrollador del sistema D:";
                    }
                }
            }

        }

    }
}