using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    // This class represents a review for a course in the tutoring academy. 
    // It includes properties such as courseId, userId, rating, and an optional comment. 
    // The review is associated with a specific course and user, allowing students to provide feedback on their learning experience.
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("courseId")]
        public string CourseId { get; set; } = null!;

        [BsonElement("userId")]
        public string UserId { get; set; } = null!;

        [BsonElement("rating")]
        public int Rating { get; set; }

        [BsonElement("comment")]
        public string? Comment { get; set; }
    }
}