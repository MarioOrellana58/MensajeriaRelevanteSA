using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TextsAPI.Models;
namespace TextsAPI.Services
{
    public class MessagesService
    {
        private readonly IMongoCollection<MessagesModel> _messages;

        public MessagesService(IMessagesDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _messages = database.GetCollection<MessagesModel>(settings.MessagesCollectionName);
        }

        public List<MessagesModel> Get(string emitter) =>
            _messages.Find(message => message.SenderReceptor.Contains(emitter)).ToList<MessagesModel>();

        public MessagesModel Create(MessagesModel message)
        {
            _messages.InsertOne(message);
            return message;
        }
        public void Update(string id, MessagesModel messageIn) =>
            _messages.ReplaceOne(message => message.Id == id, messageIn);

        public void Remove(MessagesModel messageIn) =>
            _messages.DeleteOne(message => message.Id == messageIn.Id);

        public void Remove(string id) =>
            _messages.DeleteOne(message => message.Id == id);

        public MessagesModel GetFile(string emitterReceptor, DateTime sentDate) =>
            _messages.Find(message => message.SenderReceptor.Contains(emitterReceptor) && message.SentDate == sentDate ).FirstOrDefault();
    }
}
