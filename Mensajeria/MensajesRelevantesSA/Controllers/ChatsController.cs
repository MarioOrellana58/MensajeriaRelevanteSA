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
            var messagesToShow = messages.getMessages(receptor + "|mario");
            if (messagesToShow != null)
            {
                ViewBag.chats = messagesToShow;
            }
            else
            {
                ViewBag.chats = messages.getMessages("mario|"+ receptor);
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