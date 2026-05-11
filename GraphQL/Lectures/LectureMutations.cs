using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Lectures;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Lectures
{
    // This class defines GraphQL mutations for creating, updating, and deleting lectures.
    [ExtendObjectType(typeof(Mutation))]
    public class LectureMutations
    {
        // This mutation allows an admin to create a new lecture. It takes a CreateLectureInput object as input and returns the created Lecture object.
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<CreateLectureResponse> CreateLectureAsync(
            CreateLectureInput input,
            [Service] IMongoDatabase database)
        {
            var lecturesCollection = database.GetCollection<Lecture>("lectures");
            var sectionsCollection = database.GetCollection<Section>("sections");
            var courseCollection = database.GetCollection<Course>("courses");
            var sectionExists = await sectionsCollection.Find(s => s.Id == input.SectionId).AnyAsync();
            var courseExists = await courseCollection.Find(c => c.Id == input.CourseId).AnyAsync();
            var lectureExists = await lecturesCollection.Find(l => l.SectionId == input.SectionId && l.Order == input.Order).AnyAsync();
            var sectionType = await sectionsCollection.Find(s => s.Id == input.SectionId).Project(s => s.Type).FirstOrDefaultAsync();

            if (!courseExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Course not found")
                    .SetCode("COURSE_NOT_FOUND")
                    .Build());
            }

            if (!sectionExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Section not found")
                    .SetCode("SECTION_NOT_FOUND")
                    .Build());
            }

            if (input.Order <= 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Lecture order must be a positive integer")
                    .SetCode("INVALID_LECTURE_ORDER")
                    .Build());
            }

             if (sectionType == SectionType.Video && string.IsNullOrEmpty(input.YoutubeEmbedId))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("YouTube embed ID is required for video lecture")
                    .SetCode("YOUTUBE_EMBED_ID_REQUIRED")
                    .Build());
            }

            if (sectionType == SectionType.Video && !string.IsNullOrEmpty(input.Content))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Content is not allowed for video lecture")
                    .SetCode("INVALID_CONTENT")
                    .Build());
            }

             if (sectionType == SectionType.Article && string.IsNullOrEmpty(input.Content))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Content is required for article lecture")
                    .SetCode("CONTENT_REQUIRED")
                    .Build());
            }

            if (sectionType == SectionType.Article && !string.IsNullOrEmpty(input.YoutubeEmbedId))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("YouTube embed ID is not allowed for article lecture")
                    .SetCode("INVALID_YOUTUBE_EMBED_ID")
                    .Build());
            }

            if (sectionType == SectionType.Quiz)
            {                
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Cannot add lecture to a quiz section")
                    .SetCode("INVALID_SECTION_TYPE")
                    .Build());
            }

             if (input.Duration <= TimeSpan.Zero)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Lecture duration must be greater than zero")
                    .SetCode("INVALID_DURATION")
                    .Build());
            }

             if (lectureExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("A lecture with the same order already exists in this section")
                    .SetCode("LECTURE_ALREADY_EXISTS")
                    .Build());
            }

            var lecture = new Lecture
            {
                CourseId = input.CourseId,
                SectionId = input.SectionId,
                Title = input.Title,
                YoutubeEmbedId = input.YoutubeEmbedId,
                Duration = input.Duration,
                Content = input.Content,
                Order = input.Order
            };

            await lecturesCollection.InsertOneAsync(lecture);
            return new CreateLectureResponse
            {
                Id = lecture.Id,
                CourseId = lecture.CourseId,
                SectionId = lecture.SectionId,
                Title = lecture.Title,
                YoutubeEmbedId = lecture.YoutubeEmbedId,
                Duration = lecture.Duration,
                Content = lecture.Content,
                Order = lecture.Order
            };
        }

        public async Task<UpdateLectureResponse> UpdateLectureAsync(
            UpdateLectureInput input,
             [Service] IMongoDatabase database)
        {
            var lecturesCollection = database.GetCollection<Lecture>("lectures");
            var lectureExists = await lecturesCollection.Find(l => l.Id == input.Id).AnyAsync();

            if (!lectureExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Lecture not found")
                    .SetCode("LECTURE_NOT_FOUND")
                    .Build());
            }

             if (input.Order <= 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Lecture order must be a positive integer")
                    .SetCode("INVALID_LECTURE_ORDER")
                    .Build());
            }

             var filter = Builders<Lecture>.Filter.Eq(l => l.Id, input.Id);
            var update = Builders<Lecture>.Update
                .Set(l => l.Title, input.Title)
                .Set(l => l.YoutubeEmbedId, input.YoutubeEmbedId)
                .Set(l => l.Duration, input.Duration)
                .Set(l => l.Content, input.Content)
                .Set(l => l.Order, input.Order);

            var result = await lecturesCollection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Lecture>{ReturnDocument = ReturnDocument.After}) ?? throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Lecture not found.")
                .SetCode("LECTURE_NOT_FOUND")
                .Build());

            return new UpdateLectureResponse
            {
                Id = result.Id,
                CourseId = result.CourseId,
                SectionId = result.SectionId,
                Title = result.Title,
                YoutubeEmbedId = result.YoutubeEmbedId,
                Duration = result.Duration,
                Content = result.Content,
                Order = result.Order
            };
        }

        public async Task<bool> DeleteLectureAsync(
            string lectureId,
            [Service] IMongoDatabase database)
        {
            var lecturesCollection = database.GetCollection<Lecture>("lectures");
            var lectureExists = await lecturesCollection.Find(l => l.Id == lectureId).AnyAsync();

            if (!lectureExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Lecture not found")
                    .SetCode("LECTURE_NOT_FOUND")
                    .Build());
            }

            var result = await lecturesCollection.DeleteOneAsync(l => l.Id == lectureId) ?? throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Lecture not found.")
                .SetCode("LECTURE_NOT_FOUND")
                .Build());

            return result.DeletedCount > 0;
        }
    }
}