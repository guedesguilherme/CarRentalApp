using Azure.Data.Tables;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalApp.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty]
        public CustomerEntity Customer { get; set; }

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var tableServiceClient = new TableServiceClient(connectionString);
            var tableClient = tableServiceClient.GetTableClient("guilhermecustomers");
            await tableClient.CreateIfNotExistsAsync();

            await tableClient.AddEntityAsync(Customer);

            return RedirectToPage("./Index");
        }
    }
}