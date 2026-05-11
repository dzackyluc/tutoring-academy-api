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
        [Authorize]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Section> GetSections([Service] IMongoDatabase database)
        {
            var sectionsCollection = database.GetCollection<Section>("sections");
            return sectionsCollection.AsExecutable();
        }
    }
}