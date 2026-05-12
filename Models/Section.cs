using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    // This class represents a section within a course in the tutoring academy. 
    // It includes properties such as courseId, title, and order to define the structure of the course content. 
    // Each section can contain multiple lectures, allowing for organized delivery of course material.
    public class Section
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("courseId")]
        public string CourseId { get; set; } = null!;

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("type")]
        [BsonRepresentation(BsonType.String)]
        public SectionType Type { get; set; }

        [BsonElement("order")]
        public int Order { get; set; }
    }

    public enum SectionType
    {
        Content,
        Quiz
    }
}