using HotChocolate.Data;
using HotChocolate.Authorization;
using MongoDB.Driver;
using TutoringAcademy.Models;
using TutoringAcademy.DTOs.Reviews;

namespace TutoringAcademy.GraphQL.Reviews
{
    // This class defines GraphQL mutations for creating, updating, and deleting reviews. It includes authorization to restrict access to certain operations based on user roles.
    [ExtendObjectType(typeof(Mutation))]
    public class ReviewMutations
    {
        // This mutation allows an authenticated user to create a new review. It takes a CreateReviewInput object as input and returns a CreateReviewResponse object.
        [Authorize]
        public async Task<CreateReviewResponse> CreateReviewAsync(
            CreateReviewInput input,
            [Service] IMongoDatabase database)
        {
            var reviewsCollection = database.GetCollection<Review>("reviews");

            var review = new Review
            {
                CourseId = input.CourseId,
                UserId = input.UserId,
                Rating = input.Rating,
                Comment = input.Comment
            };

            await reviewsCollection.InsertOneAsync(review);
            return new CreateReviewResponse
            {
                CourseId = review.CourseId,
                UserId = review.UserId,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }

        // This mutation allows an authenticated user to update an existing review. It takes an UpdateReviewInput object as input and returns an UpdateReviewResponse object.
        [Authorize]
        public async Task<UpdateReviewResponse> UpdateReviewAsync(
            UpdateReviewInput input,
            [Service] IMongoDatabase database)
        {
            var reviewsCollection = database.GetCollection<Review>("reviews");

            var filter = Builders<Review>.Filter.Eq(r => r.Id, input.Id);
            var update = Builders<Review>.Update
                .Set(r => r.Rating, input.Rating)
                .Set(r => r.Comment, input.Comment);

            var result = await reviewsCollection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Review>{ReturnDocument = ReturnDocument.After}) ?? throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Review not found.")
                .SetCode("REVIEW_NOT_FOUND")
                .Build());

            return new UpdateReviewResponse
            {
                Id = result.Id,
                Rating = result.Rating,
                Comment = result.Comment
            };
        }

        // This mutation allows an authenticated user to delete a review by its ID. It returns a boolean indicating whether the deletion was successful.
        [Authorize]
        public async Task<bool> DeleteReviewAsync(
            string id,
            [Service] IMongoDatabase database)
        {
            var reviewsCollection = database.GetCollection<Review>("reviews");
            var result = await reviewsCollection.DeleteOneAsync(r => r.Id == id) ?? throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Review not found.")
                .SetCode("REVIEW_NOT_FOUND")
                .Build());
            return result.DeletedCount > 0;
        }
    }
}