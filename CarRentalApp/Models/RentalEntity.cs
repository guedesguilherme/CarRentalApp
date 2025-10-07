using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Models
{
    public class RentalEntity : ITableEntity
    {
        [Required(ErrorMessage = "You must select a customer.")]
        public string PartitionKey { get; set; } // Customer's Email (RowKey from CustomerEntity)

        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string? VehiclePlate { get; set; }
        public string? VehiclePartitionKey { get; set; }

        [Required(ErrorMessage = "The Start Date is required.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "The End Date is required.")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "The Total Cost is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than zero.")]
        public double? TotalCost { get; set; }
    }
}