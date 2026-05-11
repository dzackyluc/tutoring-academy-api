using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Quizzes
{
    // This class represents the response data returned when creating a quiz. It includes the created Quiz object.
    public class CreateQuizResponse
    {
        public string Id { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string SectionId { get; set; } = null!;
        public QuizType Type { get; set; }
        public int Order { get; set; }
        public List<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; } = [];
        public List<TrueFalseQuestion> TrueFalseQuestions { get; set; } = [];
        public List<ShortAnswerQuestion> ShortAnswerQuestions { get; set; } = [];
    }
}