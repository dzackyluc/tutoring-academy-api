namespace TutoringAcademy.DTOs.Reviews
{
    // This class represents the response data returned after creating a review. It includes properties such as course ID, user ID, rating, and comment.
    public class CreateReviewResponse
    {
        public string CourseId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}