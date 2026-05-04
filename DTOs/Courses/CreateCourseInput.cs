using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Courses
{
    public class CreateCourseInput
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public double Price { get; set; }
        public string TutorId { get; set; } = null!;
        public CourseLevel Level { get; set; }
        public bool IsFree { get; set; }
        public CourseStatus Status { get; set; }
        public int TotalSections { get; set; }
        public int TotalLectures { get; set; }
        public int TotalDuration { get; set; }
    }
}