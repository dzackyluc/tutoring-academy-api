using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Enrollments
{
    // This class represents the response data for an enrollment operation. It includes properties such as Id, CourseId, UserId, and EnrolledAt, which provide information about the enrollment record created in the system.
    public class EnrollResponse
    {
        public string Id { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ProductType ProductType { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public EnrollmentStatus Status { get; set; }
        public string MidtransUrl { get; set; } = null!;
    }
}