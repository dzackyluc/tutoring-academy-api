using HotChocolate.Authorization;
using Microsoft.Extensions.Options;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Batches;
using TutoringAcademy.Models;
using TutoringAcademy.Settings;

namespace TutoringAcademy.GraphQL.Batches
{
    // This class defines GraphQL mutations for managing batches. It includes authorization and data manipulation capabilities to allow authenticated users to create, update, and delete batch records in the database.
    [ExtendObjectType(typeof(Mutation))]
    public class BatchMutations
    {
        // This mutation allows authenticated users to create a new batch. It takes input data for the batch and inserts it into the database, returning the created batch information.
        [Authorize]
        public async Task<CreateBatchResponse> CreateBatchAsync(
            CreateBatchInput input,
            [Service] IMongoDatabase database)
        {
            var batchesCollection = database.GetCollection<Batch>("batches");
            
            var batch = new Batch
            {
                CourseId = input.CourseId,
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                Capacity = input.Capacity,
                Status = BatchStatus.Available
            };

            await batchesCollection.InsertOneAsync(batch);

            return new CreateBatchResponse
            {
                Id = batch.Id,
                CourseId = batch.CourseId,
                StartDate = batch.StartDate,
                EndDate = batch.EndDate,
                Capacity = batch.Capacity,
                Status = batch.Status
            };
        }

        public async Task<UpdateBatchResponse> UpdateBatchAsync(
            UpdateBatchInput input,
            [Service] IMongoDatabase database)
        {
            var batchesCollection = database.GetCollection<Batch>("batches");
            var batch = await batchesCollection.Find(b => b.Id == input.Id).FirstOrDefaultAsync();

            if (batch == null)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Batch not found")
                    .SetCode("BATCH_NOT_FOUND")
                    .Build());
            }

            var update = Builders<Batch>.Update
                .Set(b => b.CourseId, input.CourseId)
                .Set(b => b.StartDate, input.StartDate)
                .Set(b => b.EndDate, input.EndDate)
                .Set(b => b.Capacity, input.Capacity)
                .Set(b => b.Status, input.Status);

            var result = await batchesCollection.FindOneAndUpdateAsync(
                b => b.Id == input.Id,
                update,
                new FindOneAndUpdateOptions<Batch> { ReturnDocument = ReturnDocument.After }) ?? throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Batch not found after update")
                    .SetCode("BATCH_NOT_FOUND_AFTER_UPDATE")
                    .Build());

            return new UpdateBatchResponse
            {
                Id = result.Id,
                CourseId = result.CourseId,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                Capacity = result.Capacity,
                Status = result.Status
            };
        }

        public async Task<bool> DeleteBatchAsync(
            string id,
            [Service] IMongoDatabase database)
        {
            var batchesCollection = database.GetCollection<Batch>("batches");
            var batchExists = await batchesCollection.Find(b => b.Id == id).AnyAsync();

            if (!batchExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Batch not found")
                    .SetCode("BATCH_NOT_FOUND")
                    .Build());
            }

            var deleteResult = await batchesCollection.DeleteOneAsync(b => b.Id == id);
            return deleteResult.DeletedCount > 0;
        }
    }
}