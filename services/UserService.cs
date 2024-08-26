using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalTestApi.Models;

namespace TechnicalTestApi.Services 
{
    // Service class to manage users within the User collection in MongoDB
    public class UserService
    {
        private readonly IMongoCollection<User> _userCollection;

        // Constructor to initialize the MongoDB collection using settings
        public UserService(IOptions<MongoDBSettings> settings) 
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _userCollection = mongoDatabase.GetCollection<User>(settings.Value.CollectionName);
        }

        // Get a specific user by their email
        public async Task<User> GetAsyncEmail(string email) => await _userCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        // Add a new user to the collection
        public async Task CreateAsync(User newUser) => await _userCollection.InsertOneAsync(newUser);

    }
}
