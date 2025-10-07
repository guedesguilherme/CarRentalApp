using Azure.Data.Tables;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<VehicleEntity> FeaturedVehicles { get; set; } = new();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var tableClient = new TableServiceClient(connectionString).GetTableClient("guilhermevehicles");
            await tableClient.CreateIfNotExistsAsync();

            // Query for available vehicles and take the first 3 to feature on the homepage.
            await foreach (var vehicle in tableClient.QueryAsync<VehicleEntity>(v => v.IsAvailable == true))
            {
                FeaturedVehicles.Add(vehicle);
                if (FeaturedVehicles.Count == 3)
                {
                    break;
                }
            }
        }
    }
}