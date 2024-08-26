using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TechnicalTestApi.Models
{
    // Represents a User document in MongoDB
    public class User
    {
        // Specifies that this property is the document's ID in MongoDB
        // The ID is stored as an ObjectId in MongoDB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Maps this property to the "email" field in the MongoDB document
        [BsonElement("email")]
        public string Email { get; set; }

        // Maps this property to the "password" field in the MongoDB document
        [BsonElement("password")]
        public string Password { get; set; }

        // Maps this property to the "notes" field in the MongoDB document
        // Represents a list of notes associated with the user
        [BsonElement("notes")]
        public List<Note> Notes { get; set; }

        // Default constructor
        public User() {}

        // Parameterized constructor to initialize a new User with email, password, and notes
        public User(string email, string password, List<Note> notes) {
            this.Email = email;
            this.Password = password; // Will be hashed upon registration
            this.Notes = notes;
        }
    }
}
