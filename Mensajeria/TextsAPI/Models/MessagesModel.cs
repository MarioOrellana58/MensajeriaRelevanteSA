using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace TextsAPI.Models
{
    public class MessagesModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("SenderReceptor")]
        public string SenderReceptor { get; set; }
        public string Message { get; set; }
        public object UploadedFile { get; set; }
        public DateTime SentDate { get; set; }
        public int PublicKey { get; set; }

        public MessagesModel()
        {
            SentDate = DateTime.Now;
        }
    }
}
