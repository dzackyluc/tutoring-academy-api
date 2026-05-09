using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Sections
{
    // This class represents the response data returned after creating a new section. It includes properties such as courseId, title, and order of the section.
    public class CreateSectionResponse
    {
        public string CourseId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int Order { get; set; }
    }
}