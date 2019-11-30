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

        public UserNode Get(string username) =>
            _Users.Find(user => user.Username == username).FirstOrDefault();

        public UserNode Create(UserNode user)
        {
            _Users.InsertOne(user);
            return user;
        }

        public void Update(string username, UserNode UserIn) =>
            _Users.ReplaceOne(user => user.Username == username, UserIn);
    
         public void Remove(UserNode userIn) =>
        _Users.DeleteOne(user => user.Username == userIn.Username);

        public void Remove(string userName) =>
        _Users.DeleteOne(user => user.Username == userName);
    }
}