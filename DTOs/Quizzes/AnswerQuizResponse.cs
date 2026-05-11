using TutoringAcademy.Models;

namespace TutoringAcademy.DTOs.Quizzes
{
    public class AnswerQuizResponse
    {
        public string QuizId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public double Score { get; set; }
        public List<MultipleChoiceAnswerResult> MultipleChoiceResults { get; set; } = [];
        public List<TrueFalseAnswerResult> TrueFalseResults { get; set; } = [];
        public List<ShortAnswerResult> ShortAnswerResults { get; set; } = [];
    }

    public class MultipleChoiceAnswerResult
    {
        public int QuestionIndex { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class TrueFalseAnswerResult
    {
        public int QuestionIndex { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class ShortAnswerResult
    {
        public int QuestionIndex { get; set; }
        public bool IsCorrect { get; set; }
    }
}