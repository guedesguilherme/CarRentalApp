using Azure.Data.Tables;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalApp.Pages.Vehicles
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<VehicleEntity> Vehicles { get; set; } = new List<VehicleEntity>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var tableServiceClient = new TableServiceClient(connectionString);
            var tableClient = new TableServiceClient(connectionString).GetTableClient("guilhermevehicles");
            await tableClient.CreateIfNotExistsAsync();

            // Query all entities from the table
            await foreach (VehicleEntity vehicle in tableClient.QueryAsync<VehicleEntity>())
            {
                Vehicles.Add(vehicle);
            }
        }
    }
}