using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Enrollments
{
    // This class defines GraphQL queries for retrieving enrollments. It includes authorization, projection, filtering, and sorting capabilities.
    [ExtendObjectType(typeof(Query))]
    public class EnrollmentQueries
    {
        // This query allows authenticated users to retrieve a list of enrollments. It supports paging, projection, filtering, and sorting.
        [Authorize]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Enrollment> GetEnrollments([Service] IMongoDatabase database)
        {
            var enrollmentsCollection = database.GetCollection<Enrollment>("enrollments");
            return enrollmentsCollection.AsExecutable();
        }
    }
}