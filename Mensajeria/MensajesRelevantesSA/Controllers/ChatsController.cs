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
        // GET: Chats
        public ActionResult Index(string receptor)
        {
            var loggedUser = SessionUserNode.getInstance.Username;
            ViewBag.chats = Messages.getAllContacts(SessionUserNode.getInstance.Username);
            var messagesToShow = Messages.getMessages(receptor + "|mario");
            var messagesToShow2 = Messages.getMessages("mario|"+ receptor);

            var conversation = new List<MessageModel>();
          
            conversation = messagesToShow.Union(messagesToShow2).ToList();

            conversation = conversation.OrderBy(message=> message.SentDate).ToList();

            if (conversation != null)
            {
                ViewBag.messages = conversation;
            }
                 ViewBag.receptor = receptor;
            return View();
        }

        public ActionResult GetChat(string Receptor)
        {   
            return RedirectToAction("Index",  new { receptor = Receptor });
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

            return View();
        }  
    }
}