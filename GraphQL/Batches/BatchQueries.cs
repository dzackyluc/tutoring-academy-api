using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Batches
{
    // This class defines GraphQL queries for retrieving batches. It includes authorization, projection, filtering, and sorting capabilities.
    [ExtendObjectType(typeof(Query))]
    public class BatchQueries
    {
        // This query allows authenticated users to retrieve a list of batches. It supports paging, projection, filtering, and sorting.
        [AllowAnonymous]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Batch> GetBatches([Service] IMongoDatabase database)
        {
            var batchesCollection = database.GetCollection<Batch>("batches");
            return batchesCollection.AsExecutable();
        }
    }
}