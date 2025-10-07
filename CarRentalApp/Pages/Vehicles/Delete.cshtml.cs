using Azure.Data.Tables;
using Azure.Storage.Blobs;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace CarRentalApp.Pages.Vehicles
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty]
        public VehicleEntity Vehicle { get; set; }

        public DeleteModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                return NotFound();
            }

            var tableClient = await GetTableClientAsync();
            Vehicle = await tableClient.GetEntityAsync<VehicleEntity>(partitionKey, rowKey);

            if (Vehicle == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. DELETE THE TABLE ENTITY
            var tableClient = await GetTableClientAsync();
            await tableClient.DeleteEntityAsync(Vehicle.PartitionKey, Vehicle.RowKey);

            // 2. DELETE THE ASSOCIATED BLOB FROM STORAGE
            if (!string.IsNullOrEmpty(Vehicle.ImageUrl))
            {
                var connectionString = _configuration.GetConnectionString("StorageAccount");
                var blobContainerClient = new BlobServiceClient(connectionString).GetBlobContainerClient("vehicleimages");

                // Extract the blob name from the full URL
                var blobName = Path.GetFileName(new Uri(Vehicle.ImageUrl).AbsolutePath);
                var blobClient = blobContainerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
            }

            return RedirectToPage("./Index");
        }

        private async Task<TableClient> GetTableClientAsync()
        {
            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var tableClient = new TableServiceClient(connectionString).GetTableClient("guilhermevehicles");
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }
    }
}