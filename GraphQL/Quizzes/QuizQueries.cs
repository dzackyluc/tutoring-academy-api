using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Quizzes
{
    // This class defines GraphQL queries for retrieving quizzes. It includes authorization, projection, filtering, and sorting capabilities.
    [ExtendObjectType(typeof(Query))]
    public class QuizQueries
    {
        // This query allows authenticated users to retrieve a list of quizzes. It supports paging, projection, filtering, and sorting.
        [Authorize]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Quiz> GetQuizzes([Service] IMongoDatabase database, [Service] IHttpContextAccessor HttpContextAccessor)
        {
            var quizzesCollection = database.GetCollection<Quiz>("quizzes");
            var userRole = HttpContextAccessor.HttpContext?.User.FindFirst("role")?.Value;
            var userId = HttpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            var quizzes = quizzesCollection.AsQueryable();
            if (userRole != "Admin" && userRole != "Tutor" && userId != null)
            {
                quizzes = quizzes.Where(q => q.Results.Any(r => r.UserId == userId));
            }
            return quizzesCollection.AsExecutable();
        }
    }
}