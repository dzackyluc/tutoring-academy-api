using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;
using TutoringAcademy.Models;

namespace TutoringAcademy.GraphQL.Payments
{
    // This class defines GraphQL queries for retrieving payment information. It includes authorization, projection, filtering, and sorting capabilities to allow authenticated users to access payment data securely and efficiently.
    [ExtendObjectType(typeof(Query))]
    public class PaymentQueries
    {
        // This query allows authenticated users to retrieve a list of payments. It supports paging, projection, filtering, and sorting to enable flexible querying of payment data.
        [Authorize]
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IExecutable<Payment> GetPayments([Service] IMongoDatabase database)
        {
            var paymentsCollection = database.GetCollection<Payment>("payments");
            return paymentsCollection.AsExecutable();
        }
    }
}