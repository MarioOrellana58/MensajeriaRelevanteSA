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
            var messagesToShow = new List<MessageModel>();
            messagesToShow = Messages.loadMessages(receptor);
            if (messagesToShow != null)
            {
                ViewBag.messages = messagesToShow;
            }
            if (receptor != null)
            {
                ViewBag.receptor = receptor;
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
            Messages.Create(LoggedUser + '|' +  Receptor, Message, file);
            return RedirectToAction("Index",  new {receptor = Receptor});
        }  
    }
}