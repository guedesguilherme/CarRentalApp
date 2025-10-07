using Azure.Data.Tables;
using Azure.Storage.Blobs;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Pages.Vehicles
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public VehicleEntity Vehicle { get; set; }

        [Required(ErrorMessage = "Please select a vehicle photo to upload.")]
        [BindProperty]
        public IFormFile ImageUpload { get; set; }

        public void OnGet()
        {
            // This method is called when the page is first loaded.
            // Nothing to do here for the create page.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. UPLOAD IMAGE TO BLOB STORAGE
            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Container name must be lowercase. Let's create it if it doesn't exist.
            var containerClient = blobServiceClient.GetBlobContainerClient("guilhermeguedes");
            await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            // Create a unique name for the blob
            var blobName = $"{Guid.NewGuid()}-{ImageUpload.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            // Upload the file stream
            await blobClient.UploadAsync(ImageUpload.OpenReadStream(), true);

            // 2. SAVE VEHICLE DATA TO TABLE STORAGE
            // Set the ImageUrl property to the public URL of the blob
            Vehicle.ImageUrl = blobClient.Uri.ToString();
            Vehicle.IsAvailable = true; // Default to available when creating

            var tableServiceClient = new TableServiceClient(connectionString);
            var tableClient = tableServiceClient.GetTableClient("guilhermevehicles");
            await tableClient.CreateIfNotExistsAsync();

            await tableClient.AddEntityAsync(Vehicle);

            // 3. REDIRECT TO THE LIST OF VEHICLES
            return RedirectToPage("./Index");
        }
    }
}