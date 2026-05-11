using System.Text.Json;
using TutoringAcademy.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace TutoringAcademy.Webhooks
{
    [ApiController]
    [Route("api/webhooks/midtrans")]
    public class MidtransWebhook(IMongoDatabase database) : ControllerBase
    {
        private readonly IMongoDatabase _database = database;

        [HttpPost]
        public async Task<IActionResult> Recieve([FromBody] JsonDocument payload)
        {
            // Extract Transaction Status and Order ID from the payload
            var transactionStatus = payload.RootElement.GetProperty("transaction_status").GetString();
            var orderId = payload.RootElement.GetProperty("order_id").GetString();

            // Log the received webhook for debugging purposes
            Console.WriteLine($"Received Midtrans Webhook: Transaction Status = {transactionStatus}, Order ID = {orderId}");
            // Update the order status in the database based on the transaction status
            var paymentsCollection = _database.GetCollection<Payment>("payments");
            var paymentFilter = Builders<Payment>.Filter.Eq(p => p.Id, orderId);
            var paymentUpdate = Builders<Payment>.Update.Set(p => p.Status, transactionStatus switch
            {
                "capture" => PaymentStatus.Recieved,
                "settlement" => PaymentStatus.Completed,
                "pending" => PaymentStatus.Pending,
                "deny" => PaymentStatus.Failed,
                "expire" => PaymentStatus.Expired,
                "cancel" => PaymentStatus.Cancelled,
                "refund" => PaymentStatus.Refunded,
                _ => PaymentStatus.Unknown
            });

            var enrollmentsCollection = _database.GetCollection<Enrollment>("enrollments");
            var enrollmentFilter = Builders<Enrollment>.Filter.Eq(e => e.PaymentId, orderId);
            var enrollmentUpdate = Builders<Enrollment>.Update.Set(e => e.Status, transactionStatus switch
            {
                "capture" => EnrollmentStatus.Active,
                "settlement" => EnrollmentStatus.Active,
                "pending" => EnrollmentStatus.Pending,
                "deny" => EnrollmentStatus.Rejected,
                "expire" => EnrollmentStatus.Cancelled,
                "cancel" => EnrollmentStatus.Cancelled,
                "refund" => EnrollmentStatus.Cancelled,
                _ => EnrollmentStatus.Pending
            });

            await paymentsCollection.UpdateOneAsync(paymentFilter, paymentUpdate);
            await enrollmentsCollection.UpdateOneAsync(enrollmentFilter, enrollmentUpdate);

            return Ok();
        }
    }
}