using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    public class Lecture
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("courseId")]
        public string CourseId { get; set; } = null!;

        [BsonElement("sectionId")]
        public string SectionId { get; set; } = null!;

        [BsonElement("type")]
        [BsonRepresentation(BsonType.String)]
        public LectureType Type { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("youtubeEmbedId")]
        public string YoutubeEmbedId { get; set; } = null!;

        [BsonElement("duration")]
        public TimeSpan Duration { get; set; }

        [BsonElement("content")]
        public string Content { get; set; } = null!;

        [BsonElement("order")]
        public int Order { get; set; }
    }

    public enum LectureType
    {
        Video,
        Article
    }
}