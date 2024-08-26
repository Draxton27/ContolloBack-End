using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TechnicalTestApi.Models
{
    // Represents a Note document in MongoDB
    public class Note
    {
        // Specifies that this property is the document's ID in MongoDB
        // The ID is stored as an ObjectId in MongoDB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } 

        // Maps this property to the "title" field in the MongoDB document
        [BsonElement("title")]
        public string Title { get; set; }

        // Maps this property to the "body" field in the MongoDB document
        [BsonElement("body")]
        public string Body { get; set; }

        // Maps this property to the "createdAt" field in the MongoDB document
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Default constructor
        public Note() {}

        // Parameterized constructor to initialize a new Note with a title and body
        public Note(string title, string body) {
            this.Title = title;
            this.Body = body;
            this.CreatedAt = DateTime.UtcNow;  // Sets the creation date to the current UTC time
        }
    }
}
