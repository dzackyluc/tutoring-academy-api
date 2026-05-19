using HotChocolate.Authorization;
using Microsoft.Extensions.Options;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Enrollments;
using System.Net.Http.Headers;
using TutoringAcademy.Models;
using TutoringAcademy.Settings;

namespace TutoringAcademy.GraphQL.Enrollments
{
    // This class defines GraphQL mutations for managing enrollments. It includes an Enroll mutation that allows authenticated users to enroll in a course by providing the necessary input data.
    [ExtendObjectType(typeof(Mutation))]
    public class EnrollmentMutations
    {
        // The HttpClient is used to make HTTP requests to external services if needed, such as for sending notifications or integrating with third-party APIs related to enrollments.
        private readonly HttpClient client = new();

        // This mutation allows authenticated users to enroll in a course. It takes an EnrollInput object as input, which contains the courseId and userId required to create an enrollment record in the system. The mutation returns an EnrollmentResponse object that provides information about the created enrollment.
        // The mutation also includes error handling to ensure that the course, batch, and user exist before creating the enrollment, and it checks for existing enrollments to prevent duplicate entries. Additionally, it integrates with a payment gateway (Midtrans) to create a payment transaction for the enrollment.
        [Authorize]
        public async Task<EnrollResponse> Enroll(
            EnrollInput input,
            [Service] IMongoDatabase database,
            IOptions<MidtransSettings> midtransSettings)
        {
            client.BaseAddress = new Uri(midtransSettings.Value.BaseUrl);
            var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{midtransSettings.Value.ApiKey}:"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var username = midtransSettings.Value.ApiKey;
            var password = "";
            var byteArray = System.Text.Encoding.ASCII.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var enrollmentsCollection = database.GetCollection<Enrollment>("enrollments");
            var batchCollection = database.GetCollection<Batch>("batches");
            var paymentsCollection = database.GetCollection<Payment>("payments");
            var courseCollection = database.GetCollection<Course>("courses");
            var userCollection = database.GetCollection<User>("users");

            var courseExists = await courseCollection.Find(c => c.Id == input.CourseId).AnyAsync();
            var batchExists = await batchCollection.Find(b => b.Id == input.BatchId && b.CourseId == input.CourseId).AnyAsync();
            var userExists = await userCollection.Find(u => u.Id == input.UserId).AnyAsync();

            if (!courseExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Course not found")
                    .SetCode("COURSE_NOT_FOUND")
                    .Build());
            }

            if (!batchExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Batch not found for the specified course")
                    .SetCode("BATCH_NOT_FOUND")
                    .Build());
            }

            if (!userExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("User not found")
                    .SetCode("USER_NOT_FOUND")
                    .Build());
            }

            if (input.ProductType == ProductType.FullPackage && string.IsNullOrEmpty(input.BatchId))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("BatchId is required for FullPackage product type")
                    .SetCode("BATCH_ID_REQUIRED")
                    .Build());
            }

            if (input.ProductType != ProductType.FullPackage && !string.IsNullOrEmpty(input.BatchId))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("BatchId should be empty for non-FullPackage product types")
                    .SetCode("BATCH_ID_NOT_ALLOWED")
                    .Build());
            }

            var existingEnrollment = await enrollmentsCollection.Find(e => e.CourseId == input.CourseId && e.UserId == input.UserId && e.BatchId == input.BatchId && e.ProductType == input.ProductType && e.Status == EnrollmentStatus.Pending).FirstOrDefaultAsync();
            if (existingEnrollment != null)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("User is already enrolled in this course with the same batch and product type")
                    .SetCode("ALREADY_ENROLLED")
                    .Build());
            }

            var Request = (new
            {
                transaction_details = new
                {
                    order_id = Guid.NewGuid().ToString(),
                    gross_amount = courseCollection.Find(c => c.Id == input.CourseId).Project(c => c.Price).FirstOrDefault()
                }
            }) ?? throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Failed to create payment request")
                    .SetCode("PAYMENT_REQUEST_FAILED")
                    .Build());

            var response = await client.PostAsJsonAsync("/snap/v1/transactions", Request);
            if (!response.IsSuccessStatusCode)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Failed to create payment transaction")
                    .SetCode("PAYMENT_TRANSACTION_FAILED")
                    .Build());
            }

            var result = await response.Content.ReadAsStringAsync();
            var jsonResponse = System.Text.Json.JsonDocument.Parse(result);

            var midtransUrl = jsonResponse.RootElement.GetProperty("redirect_url").GetString() ?? throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Failed to retrieve payment URL")
                    .SetCode("PAYMENT_URL_FAILED")
                    .Build());
            
            var payment = new Payment
            {
                UserId = input.UserId,
                BatchId = input.BatchId,
                Amount = courseCollection.Find(c => c.Id == input.CourseId).Project(c => c.Price).FirstOrDefault(),
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Midtrans",
                Status = PaymentStatus.Pending
            };
            
            var enrollment = new Enrollment
            {
                CourseId = input.CourseId,
                BatchId = input.BatchId,
                UserId = input.UserId,
                ProductType = input.ProductType,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Pending
            };


            // Decrement batch capacity
            var batchUpdate = Builders<Batch>.Update.Inc(b => b.Capacity, -1);
            var batchResult = await batchCollection.UpdateOneAsync(b => b.Id == input.BatchId && b.Capacity > 0, batchUpdate);

            // Check if batch capacity is now 0 and status is Available, then update status to Full
            var updatedBatch = await batchCollection.Find(b => b.Id == input.BatchId).FirstOrDefaultAsync();
            if (updatedBatch != null && updatedBatch.Capacity == 0 && updatedBatch.Status == BatchStatus.Available)
            {
                var statusUpdate = Builders<Batch>.Update.Set(b => b.Status, BatchStatus.Full);
                await batchCollection.UpdateOneAsync(b => b.Id == input.BatchId, statusUpdate);
            }

            // update user's enrolled courses
            var userUpdate = Builders<User>.Update.Push(u => u.EnrolledCourses, input.CourseId);
            await userCollection.UpdateOneAsync(u => u.Id == input.UserId, userUpdate);

            await paymentsCollection.InsertOneAsync(payment);
            await enrollmentsCollection.InsertOneAsync(enrollment);

            return new EnrollResponse
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                UserId = enrollment.UserId,
                ProductType = enrollment.ProductType,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                MidtransUrl = midtransUrl
            };
        }

        public async Task<UpdateEnrollmentStatusResponse> UpdateEnrollmentStatus(
            UpdateEnrollmentStatusInput input,
             [Service] IMongoDatabase database)
        {
            var enrollmentsCollection = database.GetCollection<Enrollment>("enrollments");
            var enrollmentExists = await enrollmentsCollection.Find(e => e.Id == input.EnrollmentId).AnyAsync();

            if (!enrollmentExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Enrollment not found")
                    .SetCode("ENROLLMENT_NOT_FOUND")
                    .Build());
            }

            var update = Builders<Enrollment>.Update.Set(e => e.Status, input.Status);
            await enrollmentsCollection.UpdateOneAsync(e => e.Id == input.EnrollmentId, update);

            var updatedEnrollment = await enrollmentsCollection.Find(e => e.Id == input.EnrollmentId).FirstOrDefaultAsync();
            return new UpdateEnrollmentStatusResponse
            {
                Id = updatedEnrollment.Id,
                CourseId = updatedEnrollment.CourseId,
                UserId = updatedEnrollment.UserId,
                EnrollmentDate = updatedEnrollment.EnrollmentDate,
                Status = updatedEnrollment.Status,
                ProductType = updatedEnrollment.ProductType
            };
        }

        public async Task<bool> CancelEnrollment(
            string enrollmentId,
             [Service] IMongoDatabase database,
             IOptions<MidtransSettings> midtransSettings)
        {
            client.BaseAddress = new Uri(midtransSettings.Value.BaseUrl);
            var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{midtransSettings.Value.ApiKey}:"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var username = midtransSettings.Value.ApiKey;
            var password = "";
            var byteArray = System.Text.Encoding.ASCII.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var enrollmentsCollection = database.GetCollection<Enrollment>("enrollments");
            var paymentsCollection = database.GetCollection<Payment>("payments");
            var enrollmentExists = await enrollmentsCollection.Find(e => e.Id == enrollmentId).AnyAsync();

            if (!enrollmentExists)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Enrollment not found")
                    .SetCode("ENROLLMENT_NOT_FOUND")
                    .Build());
            }
            
            var request = new
            {
                refund_key = enrollmentId,
                amount = paymentsCollection.Find(p => p.BatchId == enrollmentId).Project(p => p.Amount).FirstOrDefault(),
                reason = "User requested cancellation"
            };

            var update = Builders<Enrollment>.Update.Set(e => e.Status, EnrollmentStatus.Cancelled);
            var result = await enrollmentsCollection.UpdateOneAsync(e => e.Id == enrollmentId, update);

            return result.ModifiedCount > 0;
        }
    }
}