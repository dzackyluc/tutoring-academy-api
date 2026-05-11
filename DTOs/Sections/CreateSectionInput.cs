using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Sections
{
    // This class represents the input data required to create a new section. It includes properties such as courseId, title, and order of the section.
    public class CreateSectionInput
    {
        public string CourseId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public SectionType Type { get; set; }
        public int Order { get; set; }
    }
}