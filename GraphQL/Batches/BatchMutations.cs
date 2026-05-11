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
    }
}