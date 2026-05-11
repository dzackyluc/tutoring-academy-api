using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Batches
{
    // This class represents the input data required to create a new batch. It includes properties such as courseId, title, and order of the batch.
    public class CreateBatchInput
    {
        public string CourseId { get; set; } = null!;
        public string TutorId { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
    }
}