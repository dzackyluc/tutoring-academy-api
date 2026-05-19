using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    // This class represents an enrollment in the tutoring academy. 
    // It includes properties such as userId, batchId, enrollmentDate, and status to define the details of an enrollment. 
    // The EnrollmentStatus enumeration defines the different statuses an enrollment can have, such as Active, Completed, and Cancelled, which can be used to manage the lifecycle of enrollments effectively.
    public class Enrollment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("userId")]
        public string UserId { get; set; } = null!;

        [BsonElement("batchId")]
        public string BatchId { get; set; } = null!;

        [BsonElement("courseId")]
        public string CourseId { get; set; } = null!;

        [BsonElement("paymentId")]
        public string PaymentId { get; set; } = null!;

        [BsonElement("enrollmentDate")]
        public DateTime EnrollmentDate { get; set; }

        [BsonElement("completionDate")]
        public DateTime? CompletionDate { get; set; }

        [BsonElement("productType")]
        public ProductType ProductType { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public EnrollmentStatus Status { get; set; }
    }

    public enum EnrollmentStatus
    {
        Pending,
        Active,
        Completed,
        Rejected,
        Cancelled
    }

    public enum ProductType
    {
        VideoOnly,
        MaterialOnly,
        VideoAndMaterial,
        FullPackage
    }
}