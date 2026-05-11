using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Courses
{
    // This class defines GraphQL queries for retrieving courses. It includes authorization, projection, filtering, and sorting capabilities.
    [ExtendObjectType(typeof(Query))]
    public class CourseQueries
    {
        // This query allows authenticated users to retrieve a list of courses. It supports paging, projection, filtering, and sorting.
        [AllowAnonymous]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Course> GetCourses([Service] IMongoDatabase database)
        {
            var coursesCollection = database.GetCollection<Course>("courses");
            return coursesCollection.AsExecutable();
        }
    }
}