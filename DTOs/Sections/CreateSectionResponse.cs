using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Sections
{
    // This class represents the response data returned after creating a new section. It includes properties such as courseId, title, and order of the section.
    public class CreateSectionResponse
    {
        public string Id { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public SectionType Type { get; set; }
        public int Order { get; set; }
    }
}