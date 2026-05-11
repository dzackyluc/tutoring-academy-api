using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Quizzes
{
    public class UpdateQuizResponse
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