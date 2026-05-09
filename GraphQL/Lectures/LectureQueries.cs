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
        // This query allows authenticated users to retrieve a list of lectures. It supports projection, filtering, and sorting.
        [Authorize]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Lecture> GetLectures([Service] IMongoDatabase database)
        {
            var lecturesCollection = database.GetCollection<Lecture>("lectures");
            return lecturesCollection.AsExecutable();
        }
    }
}