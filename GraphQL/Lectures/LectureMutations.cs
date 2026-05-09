using Amazon.S3;
using Amazon.S3.Model;
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

             if (input.Type == LectureType.Video && string.IsNullOrEmpty(input.YoutubeEmbedId))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("YouTube embed ID is required for video lecture")
                    .SetCode("YOUTUBE_EMBED_ID_REQUIRED")
                    .Build());
            }

             if (input.Type == LectureType.Article && string.IsNullOrEmpty(input.Content))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Content is required for article lecture")
                    .SetCode("CONTENT_REQUIRED")
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
                Type = input.Type,
                YoutubeEmbedId = input.YoutubeEmbedId,
                Duration = input.Duration,
                Content = input.Content,
                Order = input.Order
            };

            await lecturesCollection.InsertOneAsync(lecture);
            return new CreateLectureResponse
            {
                CourseId = lecture.CourseId,
                SectionId = lecture.SectionId,
                Title = lecture.Title,
                Type = lecture.Type,
                YoutubeEmbedId = lecture.YoutubeEmbedId,
                Duration = lecture.Duration,
                Content = lecture.Content,
                Order = lecture.Order
            };
        }
    }
}