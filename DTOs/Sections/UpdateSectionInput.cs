using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Sections
{
    // This class represents the input data required to update an existing section. It includes properties such as the section ID, course ID, title, and order of the section.
    public class UpdateSectionInput
    {
        public string Id { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int Order { get; set; }
    }
}