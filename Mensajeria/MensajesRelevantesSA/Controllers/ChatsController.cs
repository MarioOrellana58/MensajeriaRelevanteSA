using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MensajesRelevantesSA.Models;

namespace MensajesRelevantesSA.Controllers
{
    public class ChatsController : Controller
    {
        // GET: Chats
        public ActionResult Index(string receptor)
        {
                 ViewBag.chats = new List<string> { "José", "Eduardo", "Mario", "Estuardo", "Diana", "Marroquin",
                 "José", "Eduardo", "Mario", "Estuardo", "Diana", "Marroquin"};
                 ViewBag.messages = new List<MessageModel>
                 {
                     new MessageModel{
                            Message = "hola",
                            SenderReceptor = "mario|tejeda",
                            UploadedFile = null,                           
                            SentDate = DateTime.Now
                         },

                     new MessageModel{
                            Message = "como vas",
                            SenderReceptor = "tejeda|mario",
                            UploadedFile = null,                           
                            SentDate = DateTime.Now
                         },

                     new MessageModel{
                            Message = "bien y vos?",
                            SenderReceptor = "mario|tejeda",
                            UploadedFile = null,                           
                            SentDate = DateTime.Now
                         },

                     new MessageModel{
                            Message = "todo bien bien",
                            SenderReceptor = "tejeda|mario",
                            UploadedFile = null,                           
                            SentDate = DateTime.Now
                         },

                     new MessageModel{
                            Message = "Me alegro papá",
                            SenderReceptor = "mario|tejeda",
                            UploadedFile = null,                           
                            SentDate = DateTime.Now
                         }
                 } ;            
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