namespace TutoringAcademy.DTOs.Reviews
{
    // This class represents the input data required for updating a review. It includes properties such as review ID, rating, and comment.
    public class UpdateReviewInput
    {
        public string Id { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}