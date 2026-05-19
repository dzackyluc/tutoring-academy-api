using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Lectures
{
    // This class defines GraphQL queries for retrieving lectures. It includes authorization, projection, filtering, and sorting capabilities.
    [ExtendObjectType(typeof(Query))]
    public class LectureQueries
    {
        // This query allows authenticated users to retrieve a list of lectures. It supports paging, projection, filtering, and sorting.
        [Authorize(Roles = new[] { "Admin", "Tutor" })]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Lecture> GetLectures([Service] IMongoDatabase database)
        {
            var lecturesCollection = database.GetCollection<Lecture>("lectures");
            return lecturesCollection.AsExecutable();
        }

        // This query allows authenticated users with the "User" role to retrieve a list of lectures for a specific course that they are enrolled in. It checks if the user is enrolled in the course before returning the lectures. It also supports paging, projection, filtering, and sorting.
        [Authorize(Roles = new[] { "User" })]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<IExecutable<Lecture>> GetAccessibleLectures(string userId, string courseId, [Service] IMongoDatabase database)
        {
            var enrollment = await database.GetCollection<Enrollment>("enrollments").Find(e => e.UserId == userId && e.CourseId == courseId && e.Status == EnrollmentStatus.Active).FirstOrDefaultAsync();

            var lecturesCollection = database.GetCollection<Lecture>("lectures");

            if (enrollment == null)
            {
                return lecturesCollection.Find(_ => false).AsExecutable();
            }

            var filter = Builders<Lecture>.Filter.Eq(l => l.CourseId, courseId);

            filter = enrollment.ProductType switch
            {
                        ProductType.VideoOnly => filter & Builders<Lecture>.Filter.Eq(l => l.Type, LectureType.Video),
                        ProductType.MaterialOnly => filter & Builders<Lecture>.Filter.Eq(l => l.Type, LectureType.Article),
                        _ => filter // VideoAndMaterials or FullPackage
            };

            return lecturesCollection.Find(filter).AsExecutable();
        }
    }
}