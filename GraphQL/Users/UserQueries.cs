using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Users
{
    // This class defines GraphQL queries for retrieving user information. It includes authorization, projection, filtering, and sorting capabilities to allow authenticated users to access user data securely and efficiently.
    [ExtendObjectType(typeof(Query))]
    public class UserQueries
    {
        // This query allows authenticated users to retrieve a list of users. It supports paging, projection, filtering, and sorting to enable flexible querying of user data.
        [Authorize]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<User> GetUsers([Service] IMongoDatabase database)
        {
            var usersCollection = database.GetCollection<User>("users");
            return usersCollection.AsExecutable();
        }
    }
}