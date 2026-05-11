using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Reviews
{
    [ExtendObjectType(typeof(Query))]
    public class ReviewQueries
    {
        // This query allows anyone, including unauthenticated users, to retrieve a list of reviews. It supports paging, projection, filtering, and sorting to enable flexible querying of review data.
        [AllowAnonymous]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Review> GetReviews([Service] IMongoDatabase database)
        {
            var reviewsCollection = database.GetCollection<Review>("reviews");
            return reviewsCollection.AsExecutable();
        }
    }
}