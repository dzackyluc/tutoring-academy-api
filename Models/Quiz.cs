using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    // This class represents a quiz section within a course in the tutoring academy. 
    // It includes properties such as courseId, title, and order to define the structure of the quiz content. 
    // Each quiz section can contain multiple questions, allowing for organized delivery of quiz material.
    // The Quiz class includes properties for the course ID, section ID, quiz type, order, and lists of different question types (multiple choice, true/false, short answer).
    public class Quiz
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("courseId")]
        public string CourseId { get; set; } = null!;

        [BsonElement("sectionId")]
        public string SectionId { get; set; } = null!;

        [BsonElement("type")]
        [BsonRepresentation(BsonType.String)]
        public QuizType Type { get; set; }

        [BsonElement("order")]
        public int Order { get; set; }

        [BsonElement("multipleChoiceQuestions")]
        public List<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; } = [];

        [BsonElement("trueFalseQuestions")]
        public List<TrueFalseQuestion> TrueFalseQuestions { get; set; } = [];

        [BsonElement("shortAnswerQuestions")]
        public List<ShortAnswerQuestion> ShortAnswerQuestions { get; set; } = [];

        [BsonElement("results")]
        public List<QuizResult> Results { get; set; } = [];
    }

    public class MultipleChoiceQuestion
    {
        [BsonElement("questionText")]
        public string QuestionText { get; set; } = null!;

        [BsonElement("options")]
        public List<string> Options { get; set; } = [];

        [BsonElement("correctOptionIndex")]
        public int CorrectOptionIndex { get; set; }
    }

    public class TrueFalseQuestion
    {
        [BsonElement("questionText")]
        public string QuestionText { get; set; } = null!;

        [BsonElement("isTrue")]
        public bool IsTrue { get; set; }

        [BsonElement("correctAnswer")]
        public bool CorrectAnswer { get; set; }
    }

    public class ShortAnswerQuestion
    {
        [BsonElement("questionText")]
        public string QuestionText { get; set; } = null!;

        [BsonElement("correctAnswer")]
        public string CorrectAnswer { get; set; } = null!;
    }

    public class QuizResult
    {
        [BsonElement("userId")]
        public string UserId { get; set; } = null!;

        [BsonElement("score")]
        public double Score { get; set; }

        [BsonElement("completedAt")]
        public DateTime CompletedAt { get; set; }
    }

    public enum QuizType
    {
        MultipleChoice,
        TrueFalse,
        ShortAnswer
    }
}