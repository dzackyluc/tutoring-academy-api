using Amazon.S3;
using Amazon.S3.Model;
using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Courses;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Courses
{
    // This class defines GraphQL mutations for creating, updating, and deleting courses.
    [ExtendObjectType(typeof(Mutation))]
    public class CourseMutations(IAmazonS3 s3Client)
    {
        // S3 client for handling thumbnail uploads
        private readonly IAmazonS3 _s3Client = s3Client;

        // S3 bucket name for storing course thumbnails
        private readonly string _bucketName = "tutoring-academy-bucket";
        
        // This mutation allows an admin to create a new course. It takes a CreateCourseInput object as input and returns a CreateCourseResponse object.
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<CreateCourseResponse> CreateCourseAsync(
            CreateCourseInput input,
            [GraphQLType(typeof(UploadType))] IFile? thumbnail,
            [Service] IMongoDatabase database)
        {
            var userCollection = database.GetCollection<User>("users");
            var coursesCollection = database.GetCollection<Course>("courses");
            var courseExists = await coursesCollection.Find(c => c.Title == input.Title).AnyAsync();
            var thumbnailUrl = "https://ui-avatars.com/api/?size=512&font-size=0.01&background=0D8ABC";
            var slug = input.Title.ToLower().Replace(" ", "-");

            if (courseExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Course with the same title already exists")
                    .SetCode("COURSE_ALREADY_EXISTS")
                    .Build());
            }
            
            if (thumbnail != null)
            {
                // Handle thumbnail upload and set the thumbnail URL in the course object
                var imageKey = $"course-thumbnails/{Guid.NewGuid()}-{thumbnail.Name}";
                using var stream = thumbnail.OpenReadStream();
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = imageKey,
                    InputStream = stream,
                    ContentType = thumbnail.ContentType
                };
                await _s3Client.PutObjectAsync(putRequest);
                thumbnailUrl = $"https://storage.czn.my.id/{_bucketName}/{imageKey}";
            }

            var course = new Course
            {
                Title = input.Title,
                Slug = slug,
                ThumbnailUrl = thumbnailUrl,
                Description = input.Description,
                ShortDescription = input.ShortDescription,
                Price = input.Price,
                Level = input.Level,
                IsFree = input.IsFree,
                Status = input.Status,
                TotalSections = input.TotalSections,
                TotalLectures = input.TotalLectures,
                TotalDuration = input.TotalDuration
            };

            await coursesCollection.InsertOneAsync(course);
            return new CreateCourseResponse
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                ShortDescription = course.ShortDescription,
                Price = course.Price,
                Level = course.Level,
                ThumbnailUrl = course.ThumbnailUrl,
                IsFree = course.IsFree,
                Status = course.Status,
                TotalSections = course.TotalSections,
                TotalLectures = course.TotalLectures,
                TotalDuration = course.TotalDuration
            };
        }

        // This mutation allows an admin to update an existing course. It takes the course ID and an UpdateCourseInput object as input and returns an UpdateCourseResponse object.
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<UpdateCourseResponse> UpdateCourseAsync(
            string id,
            UpdateCourseInput input,
            [Service] IMongoDatabase database)
        {
            var coursesCollection = database.GetCollection<Course>("courses");

            var update = Builders<Course>.Update
                .Set(c => c.Title, input.Title)
                .Set(c => c.Description, input.Description)
                .Set(c => c.ShortDescription, input.ShortDescription)
                .Set(c => c.Price, input.Price)
                .Set(c => c.Level, input.Level)
                .Set(c => c.IsFree, input.IsFree)
                .Set(c => c.Status, input.Status)
                .Set(c => c.TotalSections, input.TotalSections)
                .Set(c => c.TotalLectures, input.TotalLectures)
                .Set(c => c.TotalDuration, input.TotalDuration);

            var result = await coursesCollection.FindOneAndUpdateAsync(
                c => c.Id == id,
                update,
                new FindOneAndUpdateOptions<Course> { ReturnDocument = ReturnDocument.After }) ?? throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Course not found")
                    .SetCode("COURSE_NOT_FOUND")
                    .Build());

            return new UpdateCourseResponse
            {
                Id = result.Id,
                Title = result.Title,
                Description = result.Description,
                ShortDescription = result.ShortDescription,
                Price = result.Price,
                Level = result.Level,
                IsFree = result.IsFree,
                Status = result.Status,
                TotalSections = result.TotalSections,
                TotalLectures = result.TotalLectures,
                TotalDuration = result.TotalDuration
            };
        }

        // This mutation allows an admin to delete an existing course. It takes the course ID as input and returns a boolean indicating whether the deletion was successful.
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<bool> DeleteCourseAsync(
            string id,
            [Service] IMongoDatabase database)
        {
            var coursesCollection = database.GetCollection<Course>("courses");
            var sectionCollection = database.GetCollection<Section>("sections");
            var lectureCollection = database.GetCollection<Lecture>("lectures");
            var courseExists = await coursesCollection.Find(c => c.Id == id).AnyAsync();

            if (!courseExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Course not found")
                    .SetCode("COURSE_NOT_FOUND")
                    .Build());
            }

            await sectionCollection.DeleteManyAsync(s => s.CourseId == id);
            await lectureCollection.DeleteManyAsync(l => l.CourseId == id);
            var result = await coursesCollection.DeleteOneAsync(c => c.Id == id);

            return result.DeletedCount > 0;
        }
    }
}