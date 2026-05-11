using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    // This class represents a batch in the tutoring academy. 
    // It includes properties such as courseId, tutorId, startDate, endDate, and status to define the details of a batch. 
    // The BatchStatus enumeration defines the different statuses a batch can have, such as Scheduled, Ongoing, Completed, and Cancelled, which can be used to manage the lifecycle of batches effectively.
    public class Batch
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("courseId")]
        public string CourseId { get; set; } = null!;

        [BsonElement("tutorId")]
        public string TutorId { get; set; } = null!;

        [BsonElement("startDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("endDate")]
        public DateTime EndDate { get; set; }

        [BsonElement("capacity")]
        public int Capacity { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public BatchStatus Status { get; set; }
    }

    public enum BatchStatus
    {
        Available,
        Full,
        Ongoing,
        Finished,
        Cancelled
    }
}