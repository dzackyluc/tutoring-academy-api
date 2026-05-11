using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Quizzes
{
    // This class represents the input data required for creating a quiz. It includes properties such as course ID, section ID, quiz type, order, and lists of different question types (multiple choice, true/false, short answer).
    public class CreateQuizInput
    {
        public string CourseId { get; set; } = null!;
        public string SectionId { get; set; } = null!;
        public QuizType Type { get; set; }
        public int Order { get; set; }
        public List<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; } = [];
        public List<TrueFalseQuestion> TrueFalseQuestions { get; set; } = [];
        public List<ShortAnswerQuestion> ShortAnswerQuestions { get; set; } = [];
    }
}