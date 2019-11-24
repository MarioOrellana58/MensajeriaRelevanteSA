using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersAPI.Models;
using MongoDB.Driver;

namespace UsersAPI.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<UserNode> _Users;

        public UsersService(IUsersDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _Users = database.GetCollection<UserNode>(settings.UsersCollectionName);
        }

        public List<UserNode> Get() =>
        _Users.Find(user => true).ToList();

        public UserNode Get(string Username) =>
            _Users.Find<UserNode>(user => user.Username == Username).FirstOrDefault();

        public UserNode Create(UserNode user)
        {
            _Users.InsertOne(user);
            return user;
        }

        public void Update(string Username, UserNode userIn) =>
            _Users.ReplaceOne(user => user.Username == Username, userIn);

        public void Remove(UserNode userIn) =>
            _Users.DeleteOne(user => user.Username == userIn.Username);

        public void Remove(string Username) =>
            _Users.DeleteOne(user => user.Username == Username);
    }
}
