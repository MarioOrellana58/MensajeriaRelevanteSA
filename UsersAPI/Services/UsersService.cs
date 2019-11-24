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
    }
}
