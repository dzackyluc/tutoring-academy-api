using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Sections;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Sections
{
    // This class defines GraphQL mutations for managing sections. It includes authorization and handles the creation of new sections.
    [ExtendObjectType(typeof(Mutation))]
    public class SectionMutations
    {
        // This mutation allows authenticated users to create a new section. It takes input data, validates it, and inserts the new section into the database.
        [Authorize (Roles = new[] { "Admin" })]
        public async Task<CreateSectionResponse> CreateSectionAsync(
            CreateSectionInput input,
             [Service] IMongoDatabase database)
        {
            var coursesCollection = database.GetCollection<Course>("courses");
            var sectionsCollection = database.GetCollection<Section>("sections");
            var sectionExists = await sectionsCollection.Find(s => s.Title == input.Title && s.CourseId == input.CourseId).AnyAsync();
            var courseExists = await coursesCollection.Find(c => c.Id == input.CourseId).AnyAsync();

            if (!courseExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Course not found")
                    .SetCode("COURSE_NOT_FOUND")
                    .Build());
            }

            if (sectionExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("A section with the same title already exists in this course")
                    .SetCode("SECTION_EXISTS")
                    .Build());
            }

            if (input.Order <= 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Section order must be a positive integer")
                    .SetCode("INVALID_SECTION_ORDER")
                    .Build());
            }

            var sectionCount = await sectionsCollection.CountDocumentsAsync(s => s.CourseId == input.CourseId);
            var order = (int)sectionCount + 1;

            var section = new Section
            {
                CourseId = input.CourseId,
                Title = input.Title,
                Order = order
            };

            await sectionsCollection.InsertOneAsync(section);

            return new CreateSectionResponse
            {
                CourseId = section.CourseId,
                Title = section.Title,
                Order = section.Order
            };
        }
    }
}