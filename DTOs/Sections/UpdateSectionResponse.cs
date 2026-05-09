using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Sections
{
    // This class represents the response returned after successfully updating a section. It contains the updated section information.
    public class UpdateSectionResponse
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int Order { get; set; }
    }
}