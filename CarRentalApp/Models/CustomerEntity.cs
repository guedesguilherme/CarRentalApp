using Azure;
using Azure.Data.Tables;

namespace CarRentalApp.Models
{
    public class CustomerEntity : ITableEntity
    {
        // We'll use a constant partition key since we don't need to group customers.
        public string PartitionKey { get; set; } = "Customer";

        // The customer's email will be the unique RowKey.
        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Your custom properties for the customer.
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
}