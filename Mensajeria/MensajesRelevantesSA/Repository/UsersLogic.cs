﻿using MensajesRelevantesSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using AlternativeProcesses;
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
                var hash256 = SHA256.Create();

                var hashedBytes = hash256.ComputeHash(Encoding.Default.GetBytes(userPassword));
                userPassword = Encoding.Default.GetString(hashedBytes);

                hashedBytes = hash256.ComputeHash(Encoding.Default.GetBytes(userAnswer));
                userAnswer = Encoding.Default.GetString(hashedBytes);

                hashedBytes = hash256.ComputeHash(Encoding.Default.GetBytes(userQuestion));
                userQuestion = Encoding.Default.GetString(hashedBytes);

                var rand = new Random();
                int privateKey = rand.Next(10,30);//ir a validar que el random sea único user


                var cipher = new CesarCipherAlgorithm();
                cipher.DictionaryAsembler("mriojeda");
                userPassword = cipher.CipherCesar(userPassword, "mriojeda");
                userAnswer = cipher.CipherCesar(userAnswer, "mriojeda");
                userQuestion = cipher.CipherCesar(userQuestion, "mriojeda");

                var user = new UserNode() { Username = userName, Password = userPassword, Answer = userAnswer, Question = userQuestion, PrivateKey = privateKey};

                var postTask = client.PostAsJsonAsync("Users", user);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var sessionCreator = new Autentication();                
                    var jwt = sessionCreator.GenerateJWT(userName);                    
                     HttpCookie objCookie = new HttpCookie("auth");
                    objCookie["jwt"] = jwt.Result;//valida la vigencia de la sesión
                    objCookie["username"] = userName;//adquiero datos
                    objCookie["pk"] = user.PrivateKey.ToString();
                    HttpContext.Current.Response.Cookies.Add(objCookie);   
                    return "200";
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

        public bool UserExist(string userName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:51209/api/Users");
                var responseTask = client.GetAsync("Users/" + userName);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public dynamic getUserByUsername(string userName)
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

                if (searchedUser.GetType().ToString() == "System.String")
                {
                    return searchedUser;
                }
                client.BaseAddress = new Uri("http://localhost:51209/api/Users");

                var hash256 = SHA256.Create();

                var hashedBytes = hash256.ComputeHash(Encoding.Default.GetBytes(newPassword));
                newPassword = Encoding.Default.GetString(hashedBytes);

                hashedBytes = hash256.ComputeHash(Encoding.Default.GetBytes(secretAnswer));
                secretAnswer = Encoding.Default.GetString(hashedBytes);

                hashedBytes = hash256.ComputeHash(Encoding.Default.GetBytes(secretQuestion));
                secretQuestion = Encoding.Default.GetString(hashedBytes);

                
                var cipher = new CesarCipherAlgorithm();
                cipher.DictionaryAsembler("mriojeda");
                var actualQuestion = cipher.DecipherCesar(searchedUser.Question, "mriojeda");
                var actualAnswer = cipher.DecipherCesar(searchedUser.Answer, "mriojeda");

                if (actualQuestion != secretQuestion || actualAnswer != secretAnswer)
                {
                    return "Tu pregunta y o respuesta no son válidas >:(";
                }
                newPassword = cipher.CipherCesar(newPassword, "mriojeda");
                var updatedUser = new UserNode() { Username = searchedUser.Username, Password = newPassword, Question = searchedUser.Question, Answer = searchedUser.Answer, PrivateKey = searchedUser.PrivateKey};

                var putTask = client.PutAsJsonAsync("Users/" + userName, updatedUser);
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
                        return result.StatusCode.ToString() + ". Revise los datos ingresados D:";
                    }
                    else
                    {
                        return result.StatusCode.ToString() + ". Contacte a un desarrollador del sistema D:";
                    }
                }
            }

        }

        public string DeleteMyAccount(string userName, string password)
        {
            var searchedUser = getUserByUsername(userName);
            if (searchedUser.GetType().ToString() == "System.String")
            {
                return searchedUser;
            }
            var hash256 = SHA256.Create();

            var hashedBytes = hash256.ComputeHash(Encoding.Default.GetBytes(password));
            password = Encoding.Default.GetString(hashedBytes);
            
            var cipher = new CesarCipherAlgorithm();
            cipher.DictionaryAsembler("mriojeda");

            if (cipher.DecipherCesar(searchedUser.Password, "mriojeda") != password)
            {
                return "Tu contraseña es incorrecta :(";
            }
            else
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:51209/api/Users");
                    var responseTask = client.DeleteAsync("Users/" + userName);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return "Usuario eliminado correctamente";
                    }
                    else
                    {
                        return result.StatusCode.ToString() + ". Contacte a un desarrollador del sistema D:";
                    }
                }
            }
        }

        public string Validate(string userName, string password)
        {
            var searchedUser = getUserByUsername(userName);
            if (searchedUser.GetType().ToString() ==  "System.String")
            {
                return searchedUser;
            }
            var hash256 = SHA256.Create();

            var hashedBytes = hash256.ComputeHash(Encoding.Default.GetBytes(password));
            password = Encoding.Default.GetString(hashedBytes);
            
            var cipher = new CesarCipherAlgorithm();
            cipher.DictionaryAsembler("mriojeda");

            if (cipher.DecipherCesar(searchedUser.Password, "mriojeda") != password)
            {
                return "Tu contraseña es incorrecta :(";
            }
            else
            {                
                var sessionCreator = new Autentication();                
                var jwt = sessionCreator.GenerateJWT(userName);
                 HttpCookie objCookie = new HttpCookie("auth");
                objCookie["jwt"] = jwt.Result;//valida la vigencia de la sesión
                objCookie["username"] = userName;//adquiero datos
                objCookie["pk"] = searchedUser.PrivateKey.ToString();
                HttpContext.Current.Response.Cookies.Add(objCookie);             
                return  "200";
            }

        }
    }
}