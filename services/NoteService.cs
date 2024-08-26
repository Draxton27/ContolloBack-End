using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalTestApi.Models;
using MongoDB.Bson;

namespace TechnicalTestApi.Services 
{
    // Service class to manage notes within the User collection in MongoDB
    public class NoteService
    {
        private readonly IMongoCollection<User> _userCollection;

        // Constructor to initialize the MongoDB collection using settings
        public NoteService(IOptions<MongoDBSettings> settings) 
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _userCollection = mongoDatabase.GetCollection<User>(settings.Value.CollectionName);
        }

        // Get all notes for a specific user by their userId
        public async Task<List<Note>> GetAsync(string userId) 
        {
            var user = await _userCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
            return user?.Notes ?? new List<Note>();  // Return user's notes or an empty list if the user is not found
        }

        // Get a specific note by its noteId for a specific user
        public async Task<Note> GetNoteAsync(string userId, string noteId) 
        {
            var user = await _userCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
            return user?.Notes.Find(note => note.Id == noteId);  // Return the note if found, otherwise null
        }

        // Create a new note for a specific user
        public async Task CreateAsync(string userId, Note newNote)
        {
            // Assign a new Id to the note if it doesn't already have one
            if (string.IsNullOrEmpty(newNote.Id))
            {
                newNote.Id = ObjectId.GenerateNewId().ToString();
            }

            // Set the creation date to the current UTC time if not provided
            if (newNote.CreatedAt == DateTime.MinValue)
            {
                newNote.CreatedAt = DateTime.UtcNow;
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Push(x => x.Notes, newNote);

            await _userCollection.UpdateOneAsync(filter, update);  // Add the new note to the user's notes array
        }

        // Update an existing note for a specific user
        public async Task UpdateAsync(string userId, string noteId, Note updatedNote)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(x => x.Id, userId),
                Builders<User>.Filter.ElemMatch(x => x.Notes, note => note.Id == noteId)
            );

            // Ensure the note ID remains the same
            updatedNote.Id = noteId;

            var update = Builders<User>.Update.Set("Notes.$", updatedNote);

            await _userCollection.UpdateOneAsync(filter, update);  // Update the specific note in the user's notes array
        }

        // Remove a note for a specific user
        public async Task RemoveAsync(string userId, string noteId) 
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.PullFilter(x => x.Notes, note => note.Id == noteId);

            await _userCollection.UpdateOneAsync(filter, update);  // Remove the note from the user's notes array
        }
    }
}
