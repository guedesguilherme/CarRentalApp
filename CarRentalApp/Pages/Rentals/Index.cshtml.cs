using Azure.Data.Tables;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalApp.Pages.Rentals
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<RentalEntity> Rentals { get; set; } = new List<RentalEntity>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var tableClient = new TableServiceClient(connectionString).GetTableClient("guilhermerentals");
            await tableClient.CreateIfNotExistsAsync();

            await foreach (RentalEntity rental in tableClient.QueryAsync<RentalEntity>())
            {
                Rentals.Add(rental);
            }
        }
    }
}