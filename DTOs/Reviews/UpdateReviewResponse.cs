namespace TutoringAcademy.DTOs.Reviews
{
    // This class represents the response data returned after updating a review. It includes properties such as course ID, user ID, rating, and comment.
    public class UpdateReviewResponse
    {
        public string Id { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}