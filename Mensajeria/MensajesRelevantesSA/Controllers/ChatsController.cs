using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MensajesRelevantesSA.Controllers
{
    public class ChatsController : Controller
    {
        // GET: Chats
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetChat(string chat)
        {
            return RedirectToAction("Index");
        }
    }
}