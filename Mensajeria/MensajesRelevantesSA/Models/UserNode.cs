using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MensajesRelevantesSA.Models
{
    public class UserNode
    {
        public string Id { get; set; }
        
        public string Username { get; set; }

        public string Password { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public int PrivateKey { get; set; }
    }
}