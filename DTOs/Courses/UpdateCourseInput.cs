using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Courses
{
    // This class represents the input data required to update an existing course. It includes properties such as title, description, price, level, and status of the course.
    public class UpdateCourseInput
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public double Price { get; set; }
        public CourseLevel Level { get; set; }
        public bool IsFree { get; set; }
        public CourseStatus Status { get; set; }
        public int TotalSections { get; set; }
        public int TotalLectures { get; set; }
        public int TotalDuration { get; set; }
    }
}