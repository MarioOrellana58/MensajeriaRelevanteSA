using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MensajesRelevantesSA.Models;
using MensajesRelevantesSA.Repository;
using Newtonsoft.Json;

namespace MensajesRelevantesSA.Controllers
{

    public class ChatsController : Controller
    {

        private MessagesLogic Messages = new MessagesLogic();
        Autentication JWT = new Autentication();
        private string LoggedUser = string.Empty;
        // GET: Chats

        public ActionResult Index(string receptor)
        {
            
             HttpCookie objRequestRead= Request.Cookies["auth"];
            if (objRequestRead!= null && objRequestRead["jwt"]!= null  && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {
                

                if (objRequestRead!=null)
                {
                    LoggedUser  =objRequestRead["username"];
                }
                ViewBag.chats = Messages.getAllContacts(LoggedUser);
                ViewBag.LoggedUser = LoggedUser;

                if (receptor != null)
                {
                    var messagesToShow = new List<MessageModel>();
                    messagesToShow = Messages.loadMessages(receptor);
                    if (messagesToShow != null)
                    {
                        ViewBag.messages = messagesToShow;
                    }
                    if (receptor != "404")
                    {
                        ViewBag.receptor = receptor;
                    }
                    else
                    {
                        ViewBag.Error = "El usuario a quien intentas enviar un mensaje no existe";
                    }
                }

                var messagesList = new List<MessageModel>();
                var jsonMessages = string.Empty;   
                var path2 = Path.Combine(Server.MapPath("~/UploadedFiles"), "messagesForView.json");
                if (System.IO.File.Exists(path2))
                {                    
                    using (StreamReader reader = System.IO.File.OpenText(path2)) 
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null) 
                        {
                            jsonMessages += line;
                        }
                        reader.Close();
                    }
                    System.IO.File.Delete(path2);
                    JsonConvert.PopulateObject(jsonMessages, messagesList);
                    ViewBag.Found = messagesList;

                }

                
                 
                 return View();               

            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult GetChat(string Receptor)
        {  
             HttpCookie objRequestRead= Request.Cookies["auth"];
            if (objRequestRead!= null && objRequestRead["jwt"]!= null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {
                return RedirectToAction("Index",  new {  receptor = Receptor});
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult GetFile(string senderReceptor, DateTime sentDate)
        {
             HttpCookie objRequestRead= Request.Cookies["auth"];
            if (objRequestRead!= null && objRequestRead["jwt"]!= null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {
                var filePath = Messages.DecompressSelectedFile(senderReceptor, sentDate);
                var fileName = string.Empty;
                for (int i = filePath.Length-1; i > -1; i--)
                {
                    if (filePath[i] == '\\')
                    {
                        i++;
                        fileName = filePath.Substring(i, filePath.Length-i);
                        break;
                    }
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        
        [HttpPost]  
        public ActionResult NewMessage(HttpPostedFileBase file, string Receptor, string Message)  
        {
             HttpCookie objRequestRead= Request.Cookies["auth"];
            if (objRequestRead!= null && objRequestRead["jwt"]!= null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {
                if (objRequestRead!=null)
                {
                    LoggedUser  =objRequestRead["username"];
                }
                var user =  new UsersLogic();
                if (user.UserExist(Receptor))
                {                
                    Messages.Create(LoggedUser + '|' +  Receptor, Message, file);
                    return RedirectToAction("Index",  new {receptor = Receptor});
                }
                else
                {
                     return RedirectToAction("Index",  new {receptor = "404"});
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public ActionResult FindMessage(string messageToFound)
        {
            HttpCookie objRequestRead= Request.Cookies["auth"];
            if (objRequestRead!= null && objRequestRead["jwt"]!= null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {
                var messagesForView = Messages.MessageThatContainsSearch(messageToFound);
                if (messagesForView == null)
                {
                    return RedirectToAction("Index");
                }
                
                var jsonList =  JsonConvert.SerializeObject(messagesForView);
                var path2 = Path.Combine(Server.MapPath("~/UploadedFiles"), "messagesForView.json");
                if (!System.IO.File.Exists(path2)) 
                {
                    using (StreamWriter sw = System.IO.File.CreateText(path2)) 
                    {
                        sw.WriteLine(jsonList);
                    }	
                }
                return RedirectToAction("Index"); 
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult ExportChats()
        {
            HttpCookie objRequestRead = Request.Cookies["auth"];
            if (objRequestRead != null && objRequestRead["jwt"] != null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {
                var loggedUser = objRequestRead["username"];
                var pathOfExportChat = Messages.ExportMyMessages();
                if (pathOfExportChat != string.Empty)
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(pathOfExportChat);
                    System.IO.File.Delete(pathOfExportChat);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, pathOfExportChat);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


    }
}