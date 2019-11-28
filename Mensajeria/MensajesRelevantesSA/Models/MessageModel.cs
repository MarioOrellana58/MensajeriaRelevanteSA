using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MensajesRelevantesSA.Models
{
    public class MessageModel
    {
        public string SenderReceptor { get; set; }
        public string  Message { get; set; }
        public HttpPostedFileBase UploadedFile { get; set; }
        public DateTime SentDate { get; set; }

        public MessageModel()
        {            
             SentDate = DateTime.Now;
        }
    }
}