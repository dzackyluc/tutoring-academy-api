using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
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