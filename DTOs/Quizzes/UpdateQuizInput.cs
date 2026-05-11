using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Quizzes
{
    public class UpdateQuizInput
    {
        public string QuizId { get; set; } = null!;
        public QuizType Type { get; set; }
        public int Order { get; set; }
        public List<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; } = [];
        public List<TrueFalseQuestion> TrueFalseQuestions { get; set; } = [];
        public List<ShortAnswerQuestion> ShortAnswerQuestions { get; set; } = [];
    }
}