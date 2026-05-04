using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Courses;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Courses
{
    // This class defines GraphQL mutations for creating, updating, and deleting courses.
    [ExtendObjectType(typeof(Mutation))]
    public class CourseMutations
    {
        // This mutation allows an admin to create a new course. It takes a CreateCourseInput object as input and returns a CreateCourseResponse object.
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<CreateCourseResponse> CreateCourseAsync(
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
                Level = input.Level,
                IsFree = input.IsFree,
                Status = input.Status,
                TotalSections = input.TotalSections,
                TotalLectures = input.TotalLectures,
                TotalDuration = input.TotalDuration
            };

            await coursesCollection.InsertOneAsync(course);
            return new CreateCourseResponse
            {
                Title = course.Title,
                Description = course.Description,
                ShortDescription = course.ShortDescription,
                Price = course.Price,
                Level = course.Level,
                IsFree = course.IsFree,
                Status = course.Status,
                TotalSections = course.TotalSections,
                TotalLectures = course.TotalLectures,
                TotalDuration = course.TotalDuration
            };
        }

        // This mutation allows an admin to update an existing course. It takes the course ID and an UpdateCourseInput object as input and returns an UpdateCourseResponse object.
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<UpdateCourseResponse> UpdateCourseAsync(
            string id,
            UpdateCourseInput input,
            [Service] IMongoDatabase database)
        {
            var coursesCollection = database.GetCollection<Course>("courses");

            var update = Builders<Course>.Update
                .Set(c => c.Title, input.Title)
                .Set(c => c.Description, input.Description)
                .Set(c => c.ShortDescription, input.ShortDescription)
                .Set(c => c.Price, input.Price)
                .Set(c => c.Level, input.Level)
                .Set(c => c.IsFree, input.IsFree)
                .Set(c => c.Status, input.Status)
                .Set(c => c.TotalSections, input.TotalSections)
                .Set(c => c.TotalLectures, input.TotalLectures)
                .Set(c => c.TotalDuration, input.TotalDuration);

            var result = await coursesCollection.FindOneAndUpdateAsync(
                c => c.Id == id,
                update,
                new FindOneAndUpdateOptions<Course> { ReturnDocument = ReturnDocument.After }) ?? throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Course not found")
                    .SetCode("COURSE_NOT_FOUND")
                    .Build());

            return new UpdateCourseResponse
            {
                Title = result.Title,
                Description = result.Description,
                ShortDescription = result.ShortDescription,
                Price = result.Price,
                Level = result.Level,
                IsFree = result.IsFree,
                Status = result.Status,
                TotalSections = result.TotalSections,
                TotalLectures = result.TotalLectures,
                TotalDuration = result.TotalDuration
            };
        }

        // This mutation allows an admin to delete an existing course. It takes the course ID as input and returns a boolean indicating whether the deletion was successful.
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> DeleteCourseAsync(
            string id,
            [Service] IMongoDatabase database)
        {
            var coursesCollection = database.GetCollection<Course>("courses");
            var result = await coursesCollection.DeleteOneAsync(c => c.Id == id) ?? throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Course not found")
                    .SetCode("COURSE_NOT_FOUND")
                    .Build());
            return result.DeletedCount > 0;
        }
    }
}