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
        public string UploadedFile { get; set; }
        public DateTime SentDate { get; set; }
        public int PublicKey { get; set; }
        public string LoggedUser { get; set; }
        public MessageModel()
        {            
             SentDate = DateTime.Now;
            HttpCookie objRequestRead= HttpContext.Current.Request.Cookies["auth"];
            if (objRequestRead!=null)
            {
                LoggedUser  =objRequestRead["username"];
            }
            
        }
    }
}