using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations; // <-- ADD THIS LINE

namespace CarRentalApp.Models
{
    public class VehicleEntity : ITableEntity
    {
        [Required(ErrorMessage = "The Make is required.")]
        public string PartitionKey { get; set; } // Make

        [Required(ErrorMessage = "The License Plate is required.")]
        [RegularExpression("^[a-zA-Z0-9-]*$", ErrorMessage = "License plate can only contain letters, numbers, and dashes.")]
        public string RowKey { get; set; } // Plate

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        [Required(ErrorMessage = "The Model is required.")]
        public string Model { get; set; }

        // This makes the Year field required and ensures it's a number.
        [Required(ErrorMessage = "The Year is required.")]
        [Range(1900, 2100, ErrorMessage = "Please enter a valid year.")]
        public int? Year { get; set; } // <-- Changed to nullable int? for better validation

        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
    }
}