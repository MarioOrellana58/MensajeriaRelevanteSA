using System;
using System.Collections.Generic;
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
        private MessagesLogic messages = new MessagesLogic();
        // GET: Chats
        public ActionResult Index(string receptor)
        {
            ViewBag.chats = new List<string>{ "José", "Eduardo", "Mario", "Estuardo", "Diana", "Marroquin",
                 "José", "Eduardo", "Mario", "Estuardo", "Diana", "Marroquin"};
            if (receptor == null) receptor = "tejeda";
            var messagesToShow = messages.getMessages(receptor + "|mario");
            var messagesToShow2 = messages.getMessages("mario|"+ receptor);

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
    }
}