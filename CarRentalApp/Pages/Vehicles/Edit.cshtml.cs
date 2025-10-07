using Azure.Data.Tables;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalApp.Pages.Vehicles
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty]
        public VehicleEntity Vehicle { get; set; }

        public EditModel(IConfiguration configuration)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var tableClient = await GetTableClientAsync();
            // Upsert will update the entity if it exists.
            await tableClient.UpsertEntityAsync(Vehicle, TableUpdateMode.Replace);

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