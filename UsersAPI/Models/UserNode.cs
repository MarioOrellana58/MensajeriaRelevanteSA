﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UsersAPI.Models
{
    public class UserNode
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        public string Password { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
        
        public int PrivateKey { get; set; }
    }
}
