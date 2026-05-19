using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    // This class represents a course in the tutoring academy. 
    // It includes properties such as instructorId, title, description, price, and status to define the course details.
    // The class also defines enumerations for course level and status to categorize the courses accordingly.
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("slug")]
        public string Slug { get; set; } = null!;

        [BsonElement("thumbnailUrl")]
        public string ThumbnailUrl { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("shortDescription")]
        public string ShortDescription { get; set; } = null!;

        [BsonElement("level")]
        [BsonRepresentation(BsonType.String)]
        public CourseLevel Level { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("isFree")]
        public bool IsFree { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public CourseStatus Status { get; set; }

        [BsonElement("totalSections")]
        public int TotalSections { get; set; }

        [BsonElement("totalLectures")]
        public int TotalLectures { get; set; }

        [BsonElement("totalDuration")]
        public int TotalDuration { get; set; }
    }

    public enum CourseLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }

    public enum CourseStatus
    {
        Draft,
        Published,
        Archived
    }
}