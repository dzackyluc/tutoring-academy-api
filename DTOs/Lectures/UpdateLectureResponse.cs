using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Lectures
{
    // This class represents the response data returned after updating an existing lecture. It includes properties such as courseId, sectionId, title, type, youtubeEmbedId, duration, content, and order of the lecture.
    public class UpdateLectureResponse
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