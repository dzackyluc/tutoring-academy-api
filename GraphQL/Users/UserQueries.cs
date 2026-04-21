using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Users
{
    [ExtendObjectType(typeof(Query))]
    public class UserQueries
    {
        [Authorize]
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