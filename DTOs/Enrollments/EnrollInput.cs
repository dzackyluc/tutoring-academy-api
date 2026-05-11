using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Enrollments
{
    // This class represents the input data required to enroll a user in a course. It includes properties such as courseId and userId, which are necessary to create an enrollment record in the system.
    public class EnrollInput
    {
        public string CourseId { get; set; } = null!;
        public string BatchId { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}