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
            if (objRequestRead!= null && objRequestRead["jwt"]!= null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {
            
                if (objRequestRead!=null)
                {
                    LoggedUser  =objRequestRead["username"];
                }
                ViewBag.chats = Messages.getAllContacts(LoggedUser);
            

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
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        [HttpGet]  
        public ActionResult NewMessage()  
        {              
             HttpCookie objRequestRead= Request.Cookies["auth"];
            if (objRequestRead!= null && objRequestRead["jwt"]!= null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {
                 return View(); 
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
        public ActionResult FindMessage(string toFoundMessage)
        {   
             HttpCookie objRequestRead= Request.Cookies["auth"];
            if (objRequestRead!= null && objRequestRead["jwt"]!= null && JWT.ValidateSession(objRequestRead["jwt"], objRequestRead["username"]))
            {   
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
    }
}