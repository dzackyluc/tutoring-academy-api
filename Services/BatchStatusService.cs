using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.Services
{
    // This service is responsible for periodically updating the status of batches in the database. It runs as a background service and checks for batches that have reached their end date, updating their status to "Finished" if necessary.
    public class BatchStatusService : BackgroundService
    {
        private readonly IMongoDatabase _database;

        public BatchStatusService(IMongoDatabase database)
        {
            _database = database;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var batchesCollection = _database.GetCollection<Batch>("batches");
                var now = DateTime.UtcNow;
                
                var filterOngoing = Builders<Batch>.Filter.And(
                    Builders<Batch>.Filter.Lte(b => b.StartDate, now),
                    Builders<Batch>.Filter.Gte(b => b.EndDate, now),
                    Builders<Batch>.Filter.Ne(b => b.Status, BatchStatus.Ongoing)
                );

                var updateOngoing = Builders<Batch>.Update.Set(b => b.Status, BatchStatus.Ongoing);
                await batchesCollection.UpdateManyAsync(filterOngoing, updateOngoing, cancellationToken: stoppingToken);

                var filterFinished = Builders<Batch>.Filter.And(
                    Builders<Batch>.Filter.Lte(b => b.EndDate, now),
                    Builders<Batch>.Filter.Ne(b => b.Status, BatchStatus.Finished)
                );

                var updateFinished = Builders<Batch>.Update.Set(b => b.Status, BatchStatus.Finished);
                await batchesCollection.UpdateManyAsync(filterFinished, updateFinished, cancellationToken: stoppingToken);

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}