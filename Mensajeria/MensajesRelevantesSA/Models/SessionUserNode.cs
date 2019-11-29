using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MensajesRelevantesSA.Models
{
    public class SessionUserNode
    {
       
        public string Username { get; set; }
        public string JWT { get; set; }
        public int PrivateKey { get; set; }
        

        static private SessionUserNode instance = new SessionUserNode();
        static public SessionUserNode getInstance { get { return instance; }  }

        private SessionUserNode()
        {
              
        }

        public void SetSessionUserNodeData(string username, string jwt, int privateKey)
        {
            Username = username;
            JWT = jwt;
            PrivateKey = privateKey;
        }


    }
}