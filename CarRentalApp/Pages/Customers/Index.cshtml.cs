using Azure.Data.Tables;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalApp.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<CustomerEntity> Customers { get; set; } = new List<CustomerEntity>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var tableClient = new TableServiceClient(connectionString).GetTableClient("guilhermecustomers");
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.CreateIfNotExistsAsync();

            await foreach (CustomerEntity customer in tableClient.QueryAsync<CustomerEntity>())
            {
                Customers.Add(customer);
            }
        }
    }
}