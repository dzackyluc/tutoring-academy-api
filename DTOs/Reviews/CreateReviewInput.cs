namespace TutoringAcademy.DTOs.Reviews
{
    // This class represents the input data required for creating a review. It includes properties such as course ID, user ID, rating, and comment.
    public class CreateReviewInput
    {
        public string CourseId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}