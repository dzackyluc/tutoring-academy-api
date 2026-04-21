using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("slug")]
        public string Slug { get; set; } = null!;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("shortDescription")]
        public string? ShortDescription { get; set; }

        [BsonElement("tutorId")]
        public string TutorId { get; set; } = null!;

        [BsonElement("level")]
        public CourseLevel Level { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("isFree")]
        public bool IsFree { get; set; }

        [BsonElement("status")]
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