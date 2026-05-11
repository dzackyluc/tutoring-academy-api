using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Lectures
{
    // This class represents the input data required to update an existing lecture. It includes properties such as the lecture ID, course ID, section ID, title, and order of the lecture.
    public class UpdateLectureInput
    {
        public string Id { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string SectionId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string YoutubeEmbedId { get; set; } = null!;
        public TimeSpan Duration { get; set; }
        public string Content { get; set; } = null!;
        public int Order { get; set; }
    }
}