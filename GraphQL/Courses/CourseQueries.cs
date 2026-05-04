using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Courses
{
    [ExtendObjectType(typeof(Query))]
    public class CourseQueries
    {
        [Authorize]
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