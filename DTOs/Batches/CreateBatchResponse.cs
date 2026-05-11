using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Batches
{
    // This class represents the response data returned after creating a new batch. It includes properties such as Id, CourseId, TutorId, StartDate, EndDate, Capacity, and Status of the batch, which are essential for confirming the successful creation of the batch and providing relevant information about it to the client.
    public class CreateBatchResponse
    {
        public string Id { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string TutorId { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public BatchStatus Status { get; set; }
    }
}