using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Enrollments
{
    // This class represents the input data required to update the status of an enrollment. It includes properties such as EnrollmentId and Status, which are necessary to identify the enrollment record and specify the new status to be set in the system.
    public class UpdateEnrollmentStatusInput
    {
        public string EnrollmentId { get; set; } = null!;
        public EnrollmentStatus Status { get; set; }
    }
}