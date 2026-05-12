using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Batches
{
    // This class represents the input data required to update an existing batch. It includes properties such as id, courseId, title, and order of the batch.
    public class UpdateBatchInput
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