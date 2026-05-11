using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Quizzes;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Quizzes
{
    // This class defines GraphQL mutations for creating, updating, and deleting quizzes. It includes authorization checks to ensure that only users with the "Admin" role can perform these operations.
    [ExtendObjectType(typeof(Mutation))]
    public class QuizMutations
    {
        // This mutation allows an admin to create a new quiz. It takes a CreateQuizInput object as input and returns the created Quiz object.
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<CreateQuizResponse> CreateQuizAsync(
            CreateQuizInput input,
            [Service] IMongoDatabase database)
        {
            var quizzesCollection = database.GetCollection<Quiz>("quizzes");
            var sectionsCollection = database.GetCollection<Section>("sections");
            var courseCollection = database.GetCollection<Course>("courses");
            var sectionExists = await sectionsCollection.Find(s => s.Id == input.SectionId).AnyAsync();
            var courseExists = await courseCollection.Find(c => c.Id == input.CourseId).AnyAsync();
            var quizExists = await quizzesCollection.Find(q => q.SectionId == input.SectionId && q.Order == input.Order).AnyAsync();
            var sectionType = await sectionsCollection.Find(s => s.Id == input.SectionId).Project(s => s.Type).FirstOrDefaultAsync();
            var quizType = input.Type;

            if (!courseExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Course not found")
                    .SetCode("COURSE_NOT_FOUND")
                    .Build());
            }

            if (!sectionExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Section not found")
                    .SetCode("SECTION_NOT_FOUND")
                    .Build());
            }

             if (input.Order <= 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Quiz order must be a positive integer")
                    .SetCode("INVALID_QUIZ_ORDER")
                    .Build());
            }

             if (quizExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("A quiz with the same order already exists in this section")
                    .SetCode("QUIZ_ALREADY_EXISTS")
                    .Build());
            }

             if (sectionType != SectionType.Quiz)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Cannot add quiz to a non-quiz section")
                    .SetCode("INVALID_SECTION_TYPE")
                    .Build());
            }

            if (quizType == QuizType.MultipleChoice && input.MultipleChoiceQuestions.Count == 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Multiple choice questions are required for multiple choice quiz")
                    .SetCode("MULTIPLE_CHOICE_QUESTIONS_REQUIRED")
                    .Build());
            }

             if (quizType == QuizType.TrueFalse && input.TrueFalseQuestions.Count == 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("True/false questions are required for true/false quiz")
                    .SetCode("TRUE_FALSE_QUESTIONS_REQUIRED")
                    .Build());
            }

             if (quizType == QuizType.ShortAnswer && input.ShortAnswerQuestions.Count == 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Short answer questions are required for short answer quiz")
                    .SetCode("SHORT_ANSWER_QUESTIONS_REQUIRED")
                    .Build());
            }

            if (quizType == QuizType.MultipleChoice && (input.TrueFalseQuestions != null && input.TrueFalseQuestions.Count > 0 || input.ShortAnswerQuestions != null && input.ShortAnswerQuestions.Count > 0))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Only multiple choice questions are allowed for multiple choice quiz")
                    .SetCode("INVALID_QUESTIONS_FOR_MULTIPLE_CHOICE_QUIZ")
                    .Build());
            }

             if (quizType == QuizType.TrueFalse && (input.MultipleChoiceQuestions != null && input.MultipleChoiceQuestions.Count > 0 || input.ShortAnswerQuestions != null && input.ShortAnswerQuestions.Count > 0))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Only true/false questions are allowed for true/false quiz")
                    .SetCode("INVALID_QUESTIONS_FOR_TRUE_FALSE_QUIZ")
                    .Build());
            }

             if (quizType == QuizType.ShortAnswer && (input.MultipleChoiceQuestions != null && input.MultipleChoiceQuestions.Count > 0 || input.TrueFalseQuestions != null && input.TrueFalseQuestions.Count > 0))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Only short answer questions are allowed for short answer quiz")
                    .SetCode("INVALID_QUESTIONS_FOR_SHORT_ANSWER_QUIZ")
                    .Build());
            }
            

            var quiz = new Quiz
            {
                Id = Guid.NewGuid().ToString(),
                CourseId = input.CourseId,
                SectionId = input.SectionId,
                Type = input.Type,
                Order = input.Order,
                MultipleChoiceQuestions = input.MultipleChoiceQuestions ?? [],
                TrueFalseQuestions = input.TrueFalseQuestions ?? [],
                ShortAnswerQuestions = input.ShortAnswerQuestions ?? []
            };

            return new CreateQuizResponse
            {
                Id = quiz.Id,
                CourseId = quiz.CourseId,
                SectionId = quiz.SectionId,
                Type = quiz.Type,
                Order = quiz.Order,
                MultipleChoiceQuestions = quiz.MultipleChoiceQuestions,
                TrueFalseQuestions = quiz.TrueFalseQuestions,
                ShortAnswerQuestions = quiz.ShortAnswerQuestions
            };
        }

        [Authorize]
        public async Task<AnswerQuizResponse> AnswerQuizAsync(
            AnswerQuizInput input,
            [Service] IMongoDatabase database)
        {
            var quizzesCollection = database.GetCollection<Quiz>("quizzes");
            var quiz = await quizzesCollection.Find(q => q.Id == input.QuizId).FirstOrDefaultAsync() ?? throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Quiz not found")
                    .SetCode("QUIZ_NOT_FOUND")
                    .Build());
            
            var totalQuestions = quiz.MultipleChoiceQuestions.Count + quiz.TrueFalseQuestions.Count + quiz.ShortAnswerQuestions.Count;
            var correctAnswers = 0;
            
            foreach (var mcAnswer in input.MultipleChoiceAnswers)
            {
                if (mcAnswer.QuestionIndex < quiz.MultipleChoiceQuestions.Count)
                {
                    var question = quiz.MultipleChoiceQuestions[mcAnswer.QuestionIndex];
                    if (mcAnswer.SelectedOptionIndex == question.CorrectOptionIndex)
                    {
                        correctAnswers++;
                    }
                }
            }

             foreach (var tfAnswer in input.TrueFalseAnswers)
            {
                if (tfAnswer.QuestionIndex < quiz.TrueFalseQuestions.Count)
                {
                    var question = quiz.TrueFalseQuestions[tfAnswer.QuestionIndex];
                    if (tfAnswer.Answer == question.CorrectAnswer)
                    {
                        correctAnswers++;
                    }
                }
            }

             foreach (var saResponse in input.ShortAnswerResponses)
            {
                if (saResponse.QuestionIndex < quiz.ShortAnswerQuestions.Count)
                {
                    var question = quiz.ShortAnswerQuestions[saResponse.QuestionIndex];
                    if (string.Equals(saResponse.Response.Trim(), question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        correctAnswers++;
                    }
                }
            }

            var quizResult = new QuizResult
            {
                UserId = input.UserId,
                Score = (double)correctAnswers / totalQuestions * 100,
                CompletedAt = DateTime.UtcNow
            };

            var update = Builders<Quiz>.Update.Push(q => q.Results, quizResult);
            await quizzesCollection.UpdateOneAsync(q => q.Id == quiz.Id, update);

            return new AnswerQuizResponse
            {
                QuizId = quiz.Id,
                UserId = input.UserId,
                Score = quizResult.Score,
                MultipleChoiceResults = [.. input.MultipleChoiceAnswers.Select(a => new MultipleChoiceAnswerResult
                {
                    QuestionIndex = a.QuestionIndex,
                    IsCorrect = a.QuestionIndex < quiz.MultipleChoiceQuestions.Count && a.SelectedOptionIndex == quiz.MultipleChoiceQuestions[a.QuestionIndex].CorrectOptionIndex
                })],
                TrueFalseResults = [.. input.TrueFalseAnswers.Select(a => new TrueFalseAnswerResult
                {
                    QuestionIndex = a.QuestionIndex,
                    IsCorrect = a.QuestionIndex < quiz.TrueFalseQuestions.Count && a.Answer == quiz.TrueFalseQuestions[a.QuestionIndex].CorrectAnswer
                })],
                ShortAnswerResults = [.. input.ShortAnswerResponses.Select(r => new ShortAnswerResult
                {
                    QuestionIndex = r.QuestionIndex,
                    IsCorrect = r.QuestionIndex < quiz.ShortAnswerQuestions.Count && string.Equals(r.Response.Trim(), quiz.ShortAnswerQuestions[r.QuestionIndex].CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase)
                })]
            };
        }

        [Authorize(Roles = new[] { "Admin" })]
        public async Task<UpdateQuizResponse> UpdateQuizAsync(
            UpdateQuizInput input,
            [Service] IMongoDatabase database)
        {
            var quizzesCollection = database.GetCollection<Quiz>("quizzes");
            var sectionsCollection = database.GetCollection<Section>("sections");
            var quiz = await quizzesCollection.Find(q => q.Id == input.QuizId).FirstOrDefaultAsync() ?? throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Quiz not found")
                    .SetCode("QUIZ_NOT_FOUND")
                    .Build());
            var sectionType = await sectionsCollection.Find(s => s.Id == quiz.SectionId).Project(s => s.Type).FirstOrDefaultAsync();
            var quizExists = await quizzesCollection.Find(q => q.SectionId == quiz.SectionId && q.Order == input.Order && q.Id != quiz.Id).AnyAsync();

            if (input.Order <= 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Quiz order must be a positive integer")
                    .SetCode("INVALID_QUIZ_ORDER")
                    .Build());
            }

            if (quizExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("A quiz with the same order already exists in this section")
                    .SetCode("QUIZ_ALREADY_EXISTS")
                    .Build());
            }
    
            if (sectionType != SectionType.Quiz)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Cannot update quiz in a non-quiz section")
                    .SetCode("INVALID_SECTION_TYPE")
                    .Build());
            }

            if (input.Type == QuizType.MultipleChoice && input.MultipleChoiceQuestions.Count == 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Multiple choice questions are required for multiple choice quiz")
                    .SetCode("MULTIPLE_CHOICE_QUESTIONS_REQUIRED")
                    .Build());
            }

             if (input.Type == QuizType.TrueFalse && input.TrueFalseQuestions.Count == 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("True/false questions are required for true/false quiz")
                    .SetCode("TRUE_FALSE_QUESTIONS_REQUIRED")
                    .Build());
            }

             if (input.Type == QuizType.ShortAnswer && input.ShortAnswerQuestions.Count == 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Short answer questions are required for short answer quiz")
                    .SetCode("SHORT_ANSWER_QUESTIONS_REQUIRED")
                    .Build());
            }

            if (input.Type == QuizType.MultipleChoice && (input.TrueFalseQuestions != null && input.TrueFalseQuestions.Count > 0 || input.ShortAnswerQuestions != null && input.ShortAnswerQuestions.Count > 0))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Only multiple choice questions are allowed for multiple choice quiz")
                    .SetCode("INVALID_QUESTIONS_FOR_MULTIPLE_CHOICE_QUIZ")
                    .Build());
            }

             if (input.Type == QuizType.TrueFalse && (input.MultipleChoiceQuestions != null && input.MultipleChoiceQuestions.Count > 0 || input.ShortAnswerQuestions != null && input.ShortAnswerQuestions.Count > 0))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Only true/false questions are allowed for true/false quiz")
                    .SetCode("INVALID_QUESTIONS_FOR_TRUE_FALSE_QUIZ")
                    .Build());
            }

             if (input.Type == QuizType.ShortAnswer && (input.MultipleChoiceQuestions != null && input.MultipleChoiceQuestions.Count > 0 || input.TrueFalseQuestions != null && input.TrueFalseQuestions.Count > 0))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Only short answer questions are allowed for short answer quiz")
                    .SetCode("INVALID_QUESTIONS_FOR_SHORT_ANSWER_QUIZ")
                    .Build());
            }

            var update = Builders<Quiz>.Update
                .Set(q => q.Type, input.Type)
                .Set(q => q.Order, input.Order)
                .Set(q => q.MultipleChoiceQuestions, input.MultipleChoiceQuestions ?? [])
                .Set(q => q.TrueFalseQuestions, input.TrueFalseQuestions ?? [])
                .Set(q => q.ShortAnswerQuestions, input.ShortAnswerQuestions ?? []);

            await quizzesCollection.UpdateOneAsync(q => q.Id == quiz.Id, update);

            return new UpdateQuizResponse
            {
                Id = quiz.Id,
                Type = input.Type,
                Order = input.Order,
                MultipleChoiceQuestions = input.MultipleChoiceQuestions ?? [],
                TrueFalseQuestions = input.TrueFalseQuestions ?? [],
                ShortAnswerQuestions = input.ShortAnswerQuestions ?? []
            };
        }

        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> DeleteQuizAsync(
            string id,
            [Service] IMongoDatabase database)
        {
            var quizzesCollection = database.GetCollection<Quiz>("quizzes");
            var result = await quizzesCollection.DeleteOneAsync(q => q.Id == id) ?? throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Quiz not found.")
                .SetCode("QUIZ_NOT_FOUND")
                .Build());
            return result.DeletedCount > 0;
        }
    }
}