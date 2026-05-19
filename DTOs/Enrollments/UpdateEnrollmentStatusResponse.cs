using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Enrollments
{
    // This class represents the response data for updating the status of an enrollment. It includes properties such as Id, CourseId, UserId, EnrollmentDate, and Status, which provide information about the updated enrollment record in the system.
    public class UpdateEnrollmentStatusResponse
    {
        public string Id { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ProductType ProductType { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public EnrollmentStatus Status { get; set; }
    }
}