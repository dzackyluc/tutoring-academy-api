namespace TutoringAcademy.DTOs.Quizzes
{
    public class AnswerQuizInput
    {
        public string QuizId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public List<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; } = [];
        public List<TrueFalseAnswer> TrueFalseAnswers { get; set; } = [];
        public List<ShortAnswerResponse> ShortAnswerResponses { get; set; } = [];
    }

    public class MultipleChoiceAnswer
    {
        public int QuestionIndex { get; set; }
        public int SelectedOptionIndex { get; set; }
    }

    public class TrueFalseAnswer
    {
        public int QuestionIndex { get; set; }
        public bool Answer { get; set; }
    }

    public class ShortAnswerResponse
    {
        public int QuestionIndex { get; set; }
        public string Response { get; set; } = null!;
    }
}