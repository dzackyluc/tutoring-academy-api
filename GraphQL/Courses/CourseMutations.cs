using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Courses
{
    [ExtendObjectType(typeof(Mutation))]
    public class CourseMutations
    {
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Course> CreateCourseAsync(
            string title,
            string description,
            [Service] IMongoDatabase database)
        {
            var coursesCollection = database.GetCollection<Course>("courses");
            var newCourse = new Course
            {
                Title = title,
                Description = description
            };
            await coursesCollection.InsertOneAsync(newCourse);
            return newCourse;
        }
    }
}