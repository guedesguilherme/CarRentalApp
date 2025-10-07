using Azure.Data.Tables;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Pages.Rentals
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        [BindProperty]
        public RentalEntity Rental { get; set; }

        [Required(ErrorMessage = "You must select a vehicle.")]
        [BindProperty]
        public string SelectedVehicle { get; set; }

        public SelectList CustomerList { get; set; }
        public SelectList VehicleList { get; set; }

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var tableServiceClient = new TableServiceClient(connectionString);

            var customersTable = tableServiceClient.GetTableClient("guilhermecustomers");
            await customersTable.CreateIfNotExistsAsync();

            var customers = new List<CustomerEntity>();
            await foreach (var customer in customersTable.QueryAsync<CustomerEntity>())
            {
                customers.Add(customer);
            }
            CustomerList = new SelectList(customers, "RowKey", "FullName");

            var vehiclesTable = tableServiceClient.GetTableClient("guilhermevehicles");
            await vehiclesTable.CreateIfNotExistsAsync();

            var availableVehicles = new List<VehicleEntity>();
            await foreach (var vehicle in vehiclesTable.QueryAsync<VehicleEntity>(v => v.IsAvailable == true))
            {
                availableVehicles.Add(vehicle);
            }
            VehicleList = new SelectList(availableVehicles.Select(v => new {
                Value = $"{v.PartitionKey}|{v.RowKey}",
                Text = $"{v.PartitionKey} {v.Model} ({v.RowKey})"
            }), "Value", "Text");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Rental.StartDate.HasValue && Rental.EndDate.HasValue && Rental.EndDate.Value < Rental.StartDate.Value)
            {
                ModelState.AddModelError("Rental.EndDate", "End date cannot be earlier than the start date.");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var connectionString = _configuration.GetConnectionString("StorageAccount");
            var tableServiceClient = new TableServiceClient(connectionString);
            var rentalsTable = tableServiceClient.GetTableClient("guilhermerentals");
            await rentalsTable.CreateIfNotExistsAsync();

            var vehicleKeys = SelectedVehicle.Split('|');
            Rental.VehiclePartitionKey = vehicleKeys[0];
            Rental.VehiclePlate = vehicleKeys[1];

            // --- FIX IS HERE ---
            // Convert the local, unspecified dates from the form to UTC for Azure.
            if (Rental.StartDate.HasValue)
            {
                Rental.StartDate = DateTime.SpecifyKind(Rental.StartDate.Value, DateTimeKind.Utc);
            }
            if (Rental.EndDate.HasValue)
            {
                Rental.EndDate = DateTime.SpecifyKind(Rental.EndDate.Value, DateTimeKind.Utc);
            }
            // --- END OF FIX ---

            await rentalsTable.AddEntityAsync(Rental);

            var vehiclesTable = tableServiceClient.GetTableClient("guilhermevehicles");
            var vehicleToUpdate = await vehiclesTable.GetEntityAsync<VehicleEntity>(Rental.VehiclePartitionKey, Rental.VehiclePlate);
            if (vehicleToUpdate != null)
            {
                vehicleToUpdate.Value.IsAvailable = false;
                await vehiclesTable.UpsertEntityAsync(vehicleToUpdate.Value);
            }

            return RedirectToPage("./Index");
        }
    }
}