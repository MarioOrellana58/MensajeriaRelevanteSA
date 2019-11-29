using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private string LoggedUser = SessionUserNode.getInstance.Username;
        //List<MessageModel> messages = new List<MessageModel>();
        // GET: Chats
        public ActionResult Index(string receptor)
        {
            
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

        public ActionResult GetChat(string Receptor)
        {   
            return RedirectToAction("Index",  new {  receptor = Receptor});
        }

        public ActionResult GetFile(string uploadedFile)
        {
            return RedirectToAction("Index");
        }

        [HttpGet]  
        public ActionResult NewMessage()  
        {  
            return View();  
        }  
        [HttpPost]  
        public ActionResult NewMessage(HttpPostedFileBase file, string Receptor, string Message)  
        {
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
    }
}