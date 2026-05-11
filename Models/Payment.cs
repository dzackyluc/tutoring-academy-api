using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("userId")]
        public string UserId { get; set; } = null!;

        [BsonElement("batchId")]
        public string BatchId { get; set; } = null!;

        [BsonElement("amount")]
        public double Amount { get; set; }

        [BsonElement("paymentDate")]
        public DateTime PaymentDate { get; set; }

        [BsonElement("paymentMethod")]
        public string PaymentMethod { get; set; } = null!;

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public PaymentStatus Status { get; set; }
    }

    public enum PaymentStatus
    {
        Recieved,
        Pending,
        Completed,
        Expired,
        Failed,
        Cancelled,
        Refunded,
        Unknown
    }
}