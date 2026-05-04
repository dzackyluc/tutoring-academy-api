using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Courses;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Courses
{
    [ExtendObjectType(typeof(Mutation))]
    public class CourseMutations
    {
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Course> CreateCourseAsync(
            CreateCourseInput input,
            [Service] IMongoDatabase database)
        {
            var coursesCollection = database.GetCollection<Course>("courses");

            var course = new Course
            {
                Title = input.Title,
                Description = input.Description,
                ShortDescription = input.ShortDescription,
                Price = input.Price,
                TutorId = input.TutorId,
                Level = input.Level,
                IsFree = input.IsFree,
                Status = input.Status,
                TotalSections = input.TotalSections,
                TotalLectures = input.TotalLectures,
                TotalDuration = input.TotalDuration
            };

            await coursesCollection.InsertOneAsync(course);
            return course;
        }
    }
}