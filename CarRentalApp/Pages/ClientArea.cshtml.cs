using Azure.Data.Tables;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalApp.Pages
{
    public class ClientAreaModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // This holds the email the user types into the search box.
        [BindProperty]
        public string SearchEmail { get; set; }

        // This will hold the list of rentals found for that email.
        public List<RentalEntity> Rentals { get; set; } = new List<RentalEntity>();

        // This flag tells our page whether a search has been done yet.
        public bool SearchPerformed { get; set; } = false;

        public ClientAreaModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            // Nothing to do when the page first loads.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(SearchEmail))
            {
                return Page();
            }

            SearchPerformed = true;

            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var tableClient = new TableServiceClient(connectionString).GetTableClient("guilhermerentals");

            // This is the key part: we query the "rentals" table using the email
            // as the PartitionKey. This is a very fast and efficient query.
            await foreach (var rental in tableClient.QueryAsync<RentalEntity>(r => r.PartitionKey == SearchEmail))
            {
                Rentals.Add(rental);
            }

            return Page();
        }
    }
}