using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Sections
{
    // This class defines GraphQL queries for retrieving sections. It includes authorization, projection, filtering, and sorting capabilities.
    [ExtendObjectType(typeof(Query))]
    public class SectionQueries
    {
        // This query allows authenticated users to retrieve a list of sections. It supports paging, projection, filtering, and sorting.
        [Authorize(Roles = new[] { "Admin", "Tutor" })]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Section> GetSections([Service] IMongoDatabase database)
        {
            var sectionsCollection = database.GetCollection<Section>("sections");
            return sectionsCollection.AsExecutable();
        }

        [Authorize(Roles = new[] { "User" })]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Section> GetAccessibleSections(string userId, [Service] IMongoDatabase database)
        {
            var sectionsCollection = database.GetCollection<Section>("sections");
            var enrollmentsCollection = database.GetCollection<Enrollment>("enrollments");

            var userEnrollments = enrollmentsCollection.Find(e => e.UserId == userId && e.Status == EnrollmentStatus.Active).ToList();
            var courseIds = userEnrollments.Select(e => e.CourseId).ToList();

            var filter = Builders<Section>.Filter.In(s => s.CourseId, courseIds);

            return sectionsCollection.Find(filter).AsExecutable();
        }
    }
}